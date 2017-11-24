using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation.Language;
using Gherkin.Ast;
using JaCoCoReader.Core.Constants;
using JaCoCoReader.Core.Models.Tests;

namespace JaCoCoReader.Core.Services
{
    public class PowerShellTestDiscoverer
    {
        public static TestProject GetTests(string source, IMessageLogger logger)
        {
            TestProject project = new TestProject
            {
                Name = Path.GetFileNameWithoutExtension(source),
                Path = source
            };

            GetTests(source, project, logger);

            return project;
        }

        private static void GetTests(string folderPath, TestFolder parentFolder, IMessageLogger logger)
        {

            foreach (string source in Directory.EnumerateDirectories(folderPath))
            {
                TestFolder folder = new TestFolder
                {
                    Name = Path.GetFileNameWithoutExtension(source),
                    Path = source
                };

                GetTests(source, folder, logger);

                if (folder.Folders.Count > 0
                    || folder.Files.Count > 0)
                {
                    parentFolder.Folders.Add(folder);
                }
            }
            foreach (string source in Directory.EnumerateFiles(folderPath, $"*{Constant.TestsPs1}"))
            {
                TestFile file = new TestFile
                {
                    Name = Path.GetFileNameWithoutExtension(source),
                    Path = source
                };
                if (DiscoverPesterTests(source, file.Describes, logger))
                {
                    if (file.Describes.Count > 0)
                    {
                        parentFolder.Files.Add(file);
                    }
                }
            }
            foreach (string source in Directory.EnumerateFiles(folderPath, $"*{Constant.Feature}"))
            {
                TestFeature feature = DiscoverPesterFeatures(source, null);
                if (feature != null)
                {
                    parentFolder.Features.Add(feature);
                }
            }
        }

        public static TestFile DiscoverTestFile(string source)
        {
            TestFile file = new TestFile
            {
                Name = Path.GetFileNameWithoutExtension(source),
                Path = source
            };
            if (DiscoverPesterTests(source, file.Describes, null))
            {
                return file;
            }
            return null;
        }

        protected static TestFeature DiscoverPesterFeatures(string source, IMessageLogger logger)
        {
            if (!File.Exists(source))
            {
                return null;
            }

            Gherkin.Parser parser = new Gherkin.Parser();

            GherkinDocument document = parser.Parse(source);

            TestFeature feature = new TestFeature
            {
                Name = Path.GetFileNameWithoutExtension(source),
                Path = source,
                Document = document

            };

            foreach (ScenarioDefinition scenario in document.Feature.Children)
            {


                if (scenario is ScenarioOutline outline)
                {
                    foreach (Examples example in outline.Examples)
                    {
                        TestScenario testScenario = new TestScenario()
                        {
                            Name = $"{scenario.Name}\n  Examples:{example.Name}",
                            Path = source,
                            Scenario = scenario
                        };
                        feature.Scenarios.Add(testScenario);
                        foreach (TableRow row in example.TableBody)
                        {
                            foreach (Step step in scenario.Steps)
                            {
                                TestStep testStep = new TestStep()
                                {
                                    Name = step.Text,
                                    Path = source,
                                    Step = step
                                };
                                testScenario.Steps.Add(testStep);
                            }
                        }
                    }
                }
                else
                {
                    TestScenario testScenario = new TestScenario()
                    {
                        Name = scenario.Name,
                        Path = source,
                        Scenario = scenario
                    };

                    feature.Scenarios.Add(testScenario);

                    foreach (Step step in scenario.Steps)
                    {
                        TestStep testStep = new TestStep()
                        {
                            Name = step.Text,
                            Path = source,
                            Step = step
                        };
                        testScenario.Steps.Add(testStep);
                    }
                }
            }
            if (feature.Scenarios.Count == 0)
            {
                return null;
            }
            return feature;
        }

        protected static bool DiscoverPesterTests(string source, TestDescribeCollection tests, IMessageLogger logger)
        {
            if (!File.Exists(source))
            {
                return false;
            }
            //SendMessage(TestMessageLevel.Informational, string.Format(Resources.SearchingForTestsFormat, source), logger);
            Token[] tokens;
            ParseError[] errors;
            ScriptBlockAst ast = Parser.ParseFile(source, out tokens, out errors);

            //if (errors.Any())
            //{
            //    foreach (var error in errors)
            //    {
            //        SendMessage(TestMessageLevel.Error, string.Format(Resources.ParserErrorFormat, error.Message), logger);
            //    }
            //    return;
            //}

            IEnumerable<Ast> testSuites =
                ast.FindAll(
                    m =>
                        (m is CommandAst) &&
                        string.Equals("describe", ((CommandAst)m).GetCommandName(), StringComparison.OrdinalIgnoreCase), true);

            foreach (Ast describeAst in testSuites)
            {
                string describeName = GetFunctionName(logger, describeAst, "describe");
                Dictionary<string, TestContext> contextByName = new Dictionary<string, TestContext>();

                //IEnumerable<string> tags = GetDescribeTags(logger, describeAst);

                IEnumerable<Ast> its = describeAst.FindAll(m => (m as CommandAst)?.GetCommandName()?.Equals("it", StringComparison.OrdinalIgnoreCase) == true, true);

                foreach (Ast test in its)
                {
                    CommandAst itAst = (CommandAst)test;
                    string itName = GetFunctionName(logger, test, "it");
                    string contextName = GetParentContextName(logger, test);

                    if (!contextByName.TryGetValue(contextName, out TestContext context))
                    {
                        context = new TestContext
                        {
                            Name = contextName,
                            Path = source
                        };
                        contextByName.Add(contextName, context);
                    }

                    // Didn't find the name for the test. Skip it.
                    if (string.IsNullOrEmpty(itName))
                    {
                        SendMessage(TestMessageLevel.Informational, "Test name was empty. Skipping test.", logger);
                        continue;
                    }

                    TestIt it = new TestIt
                    {
                        Name = itName,
                        Ast = itAst,
                        Path = source,
                        LineNr = itAst.Extent.StartLineNumber

                    };

                    context.Its.Add(it);
                }
                if (contextByName.Count > 0)
                {
                    TestDescribe describe = new TestDescribe
                    {
                        Name = describeName,
                        Ast = describeAst,
                        Path = source,
                        LineNr = describeAst.Extent.StartLineNumber
                    };
                    foreach (TestContext context in contextByName.Values)
                    {
                        describe.Contexts.Add(context);
                    }
                    tests.Add(describe);
                }
            }
            return true;
        }

        private static string GetParentContextName(IMessageLogger logger, Ast ast)
        {
            if (ast.Parent is CommandAst commandAst &&
                string.Equals("context", commandAst.GetCommandName(), StringComparison.OrdinalIgnoreCase))
            {
                return GetFunctionName(logger, ast.Parent, "context");
            }

            if (ast.Parent != null)
            {
                return GetParentContextName(logger, ast.Parent);
            }

            return "No Context";
        }

        private static string GetFunctionName(IMessageLogger logger, Ast context, string functionName)
        {
            CommandAst contextAst = (CommandAst)context;
            string contextName = string.Empty;
            bool nextElementIsName1 = false;
            bool skipElement = false;
            foreach (CommandElementAst element in contextAst.CommandElements)
            {
                if (skipElement)
                {
                    skipElement = false;
                    continue;
                }

                if (element is StringConstantExpressionAst &&
                    !(element as StringConstantExpressionAst).Value.Equals(functionName,
                        StringComparison.OrdinalIgnoreCase))
                {
                    contextName = (element as StringConstantExpressionAst).Value;
                    break;
                }

                if (nextElementIsName1 && element is StringConstantExpressionAst)
                {
                    contextName = (element as StringConstantExpressionAst).Value;
                    break;
                }

                if (element is CommandParameterAst)
                {
                    if ((element as CommandParameterAst).ParameterName.Equals("Name",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        nextElementIsName1 = true;
                    }
                    else
                    {
                        skipElement = true;
                    }
                }
            }

            return contextName;
        }

        private static IEnumerable<string> GetDescribeTags(IMessageLogger logger, Ast context)
        {
            CommandAst contextAst = (CommandAst)context;
            string contextName = string.Empty;
            bool nextElementIsName1 = false;
            foreach (CommandElementAst element in contextAst.CommandElements)
            {
                if (nextElementIsName1)
                {
                    IEnumerable<StringConstantExpressionAst> tagStrings = element.FindAll(m => m is StringConstantExpressionAst, true)
                        .OfType<StringConstantExpressionAst>();
                    foreach (StringConstantExpressionAst tag in tagStrings)
                    {
                        yield return tag.Value;
                    }
                    break;
                }

                if (element is CommandParameterAst &&
                    "tags".Contains((element as CommandParameterAst).ParameterName.ToLower()))
                {
                    nextElementIsName1 = true;
                }
            }
        }

        private static void SendMessage(TestMessageLevel level, string message, IMessageLogger logger)
        {
            logger?.SendMessage(level, message);
        }
    }
}