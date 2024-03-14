using System;

[Serializable]
public struct BlackboardBool
{
    public string Name;
    public bool Enabled;
    public void SetEnabled(bool b)
    {
        Enabled = b;
    }
}
