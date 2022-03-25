public class ToastRequest
{
    public string message;
    public float duration;
    public bool isPauseable;

    public ToastRequest(string s, float _duration = 0.5f, bool _isPauseable = false)
    {
        message = s;
        duration = _duration;
        isPauseable = _isPauseable;
    }

    public override string ToString()
    {
        return "Toast set to : " + message;
    }
}