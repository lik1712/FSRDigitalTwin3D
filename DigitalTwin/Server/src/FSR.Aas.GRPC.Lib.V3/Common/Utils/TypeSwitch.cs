namespace FSR.Aas.GRPC.Lib.V3.Common.Utils;

public class TypeSwitch {
    private readonly object? _obj;
    
    private TypeSwitch(object? obj) => _obj = obj;

    public static TypeSwitch From(object? obj) {
        return new TypeSwitch(obj);
    }

    public TypeSwitch Case<T>(Action<T> action) {
        if (_obj is T t) {
            action(t);
        }
        return this;
    }
}

public class TypeSwitch<TState> {
    private readonly object? _obj;
    private readonly TState _state;
    
    private TypeSwitch(object? obj, TState state) {
        _obj = obj;
        _state = state;
    }

    public static TypeSwitch<TState> From(object? obj, TState state) {
        return new TypeSwitch<TState>(obj, state);
    }

    public TypeSwitch<TState> Case<T>(Action<T, TState> action) {
        if (_obj is T t) {
            action(t, _state);
        }
        return this;
    }
}