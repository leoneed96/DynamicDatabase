using ConsoleApp1.Actions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace ConsoleApp1
{
    public class DynamicDatabase : DynamicObject
    {
        private readonly DataContext _dataContext;

        private readonly Dictionary<string, (Func<object> getter, PropertyInfo[] properties)> DbSets =
            new Dictionary<string, (Func<object> getter, PropertyInfo[] properties)>();

        private void Init()
        {
            // retreiving props of type DbSet which is generic & abstract since we need DbSet<T>
            foreach (var prop in _dataContext.GetType().GetProperties()
                .Where(x => x.PropertyType.IsGenericType && x.PropertyType.IsAbstract))
            {
                var entityType = prop.PropertyType.GenericTypeArguments[0];

                DbSets.Add(prop.Name.ToLower(), (Helper.GenerateGetter(prop.Name, _dataContext), entityType.GetProperties()));
            }
        }

        public DynamicDatabase(DataContext context)
        {
            _dataContext = context;
            Init();
        }

        // We have only properties
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = null;
            return false;
        }

        // access property
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var name = binder.Name.ToLower();

            if (DbSets.TryGetValue(name, out var data))
            {
                result = new DynamicTable(data);
                return true;
            }

            result = null;
            return false;
        }
    }
}
