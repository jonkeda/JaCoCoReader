function Calculate-Numbers($a, $o, $b) {
    if ($o -eq '+')
    {
        return $a + $b
    }
    if ($o -eq '*')
    {
        return $a * $b
    }
    return $a - $b
}
