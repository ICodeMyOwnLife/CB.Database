using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CB.Model.Common;


namespace CB.Database.EntityFramework
{
    public class EnitityDependentInfo
    {
        #region  Properties & Indexers
        public PropertyInfo DependentPropertyIdInfo { get; set; }
        public PropertyInfo DependentPropertyInfo { get; set; }
        #endregion
    }

    public class EntityManager
    {
        #region Fields
        private readonly Dictionary<Type, List<EnitityDependentInfo>> _dependentDictionary =
            new Dictionary<Type, List<EnitityDependentInfo>>();
        #endregion


        #region Methods
        public void AddDependent<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> dependentPropertyExpression,
            Expression<Func<TEntity, int>> dependentPropertyIdExpression) where TEntity: IIdEntity
            where TProperty: IIdEntity
            =>
                AddDependent(typeof(TEntity), dependentPropertyExpression.GetPropertyInfo(),
                    dependentPropertyIdExpression.GetPropertyInfo());

        public IEnumerable<string> GetDependentPropertyNames<TEntity>(TEntity entity)
        {
            List<EnitityDependentInfo> dependentInfos;
            return TryGetDependentInfos<TEntity>(out dependentInfos)
                       ? dependentInfos.Select(i => i.DependentPropertyInfo.Name) : new string[0];
        }

        // UNDONE: Reset Properties, Set many-to-many
        public void SetDependent<TEntity>(TEntity entity) where TEntity: IIdEntity
        {
            List<EnitityDependentInfo> dependentInfos;
            if (!TryGetDependentInfos<TEntity>(out dependentInfos)) return;

            foreach (var dependentInfo in dependentInfos)
            {
                var property = dependentInfo.DependentPropertyIdInfo.GetValue(entity) as IIdEntity;
                if (property == null) continue;

                dependentInfo.DependentPropertyInfo.SetValue(entity, null);
                dependentInfo.DependentPropertyIdInfo.SetValue(entity, property.Id);
            }
        }
        #endregion


        #region Implementation
        private void AddDependent(Type entityType, PropertyInfo dependentPropertyInfo,
            PropertyInfo dependentPropertyIdInfo)
        {
            List<EnitityDependentInfo> dependentList;

            if (!_dependentDictionary.ContainsKey(entityType))
            {
                dependentList = new List<EnitityDependentInfo>();
                _dependentDictionary[entityType] = dependentList;
            }
            else
            {
                dependentList = _dependentDictionary[entityType];
            }
            dependentList.Add(new EnitityDependentInfo
            {
                DependentPropertyInfo = dependentPropertyInfo,
                DependentPropertyIdInfo = dependentPropertyIdInfo
            });
        }

        private bool TryGetDependentInfos<TEntity>(out List<EnitityDependentInfo> dependentInfos)
        {
            return _dependentDictionary.TryGetValue(typeof(TEntity), out dependentInfos);
        }
        #endregion
    }
}