using System;

namespace OM
{
    public class OMSimpleStateMachine<T>
    {
        public event Action<T, T> OnStateChanged; 
        
        public T LastState { get; private set; }
        public T CurrentState { get; private set; }
        
        public OMSimpleStateMachine(T initialState,Action<T,T> onStateChanged = null)
        {
            if(onStateChanged != null) OnStateChanged += onStateChanged;
            SetNewState(initialState);
        }

        public void SetNewState(T newState,bool force = false)
        {
            if(!force && CurrentState != null && CurrentState.Equals(newState)) return;
            LastState = CurrentState;
            CurrentState = newState;
            OnStateChanged?.Invoke(CurrentState,LastState);
        }

        public void OnChange(Action<T,T> callback)
        {
            OnStateChanged += callback;
        }
        
    }
}