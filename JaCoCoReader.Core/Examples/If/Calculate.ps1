function Calculate-Numbers($a, $o, $b) {
    if ($o -eq '+')
    {
        return $a + $b
    }
    else
    {
        return $a - $b
    }
}
