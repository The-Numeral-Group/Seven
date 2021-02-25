/*CLASS WITH NO DATA
but seriously this class is just a standard unity event, except
it takes 1 argument of type GameObject in its Invoke function.
This is definied as the generic part of UnityEvent, so no code
is needed here, except for the class declaration*/

public class ObjectSelfEditEvent : UnityEngine.Events.UnityEvent<UnityEngine.GameObject> {}