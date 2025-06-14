using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OtusSpaceBattle.Infrastructure
{
    public static class IoC
    {
        private class Scope
        {
            public readonly ConcurrentDictionary<string, Func<object[], object>> Registrations = new();
        }

        private static readonly ConcurrentDictionary<string, Scope> _scopes = new();
        private static readonly ThreadLocal<string> _currentScope = new(() => "root");
        static IoC()
        {
            _scopes.TryAdd("root", new Scope());
        }

        public static T Resolve<T>(string key, params object[] args)
        {
            return (T)Resolve(key, args);
        }

        public static object Resolve(string key, params object[] args)
        {
            var scope = _scopes[_currentScope.Value];
            switch (key)
            {
                case "IoC.Register":
                    // args: [string regKey, Func<object[], object> factory]
                    var regKey = (string)args[0];
                    var factory = (Func<object[], object>)args[1];
                    scope.Registrations[regKey] = factory;
                    return new Action(() => { });
                case "Scopes.New":
                    // args: [string scopeId]
                    var newScopeId = (string)args[0];
                    _scopes.TryAdd(newScopeId, new Scope());
                    return new Action(() => { });
                case "Scopes.Current":
                    // args: [string scopeId]
                    var curScopeId = (string)args[0];
                    if (!_scopes.ContainsKey(curScopeId))
                        throw new InvalidOperationException($"Scope '{curScopeId}' does not exist.");
                    _currentScope.Value = curScopeId;
                    return new Action(() => { });
                default:
                    if (scope.Registrations.TryGetValue(key, out var creator))
                        return creator(args);
                    throw new InvalidOperationException($"No registration for key '{key}' in scope '{_currentScope.Value}'.");
            }
        }
    }
}
