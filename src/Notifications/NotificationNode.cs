namespace Blindness.Notifications;

public class NotificationNode
{
    public void Notify()
    {

    }
}

public class NotificationNode<T> : NotificationNode
{
    private T crrValue;
    private T oldValue;
    
    public T Value
    {
        get => crrValue;
        set
        {
            crrValue = value;
            if (crrValue != oldValue)
                Notify();
            oldValue = value;
        }
    }
}