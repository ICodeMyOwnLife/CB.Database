using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CB.Model.Common;


namespace CB.Database.EntityFramework
{
    public class DependentPropertyDictionary
    {
        #region Fields
        readonly IDictionary<Type, IList<ModelInfo>> _dependentDictionary =
            new Dictionary<Type, IList<ModelInfo>>();
        #endregion


        #region Methods
        public void Add<TModel>(PropertyInfo property, PropertyInfo id, PropertyInfo propertyId)
            => Add(typeof(TModel), property, id, propertyId);

        public void Add<TModel, TProperty, TId>(Expression<Func<TModel, TProperty>> propertyExpression,
            Expression<Func<TModel, TId>> idExpression, Expression<Func<TProperty, TId>> propertyIdExpression)
            =>
                Add<TModel>(propertyExpression.GetPropertyInfo(), idExpression.GetPropertyInfo(),
                    propertyIdExpression.GetPropertyInfo());

        public IEnumerable<object> GetProperties<TModel>(TModel model)
        {
            if (model == null) return null;
            var modelType = model.GetType();
            return _dependentDictionary.ContainsKey(modelType)
                       ? _dependentDictionary[modelType].Select(mi => mi.GetProperty(model)) : null;
        }
        #endregion


        #region Implementation
        private void Add(Type type, PropertyInfo property, PropertyInfo id, PropertyInfo propertyId)
        {
            IList<ModelInfo> dependentProperties;

            if (!_dependentDictionary.ContainsKey(type))
            {
                dependentProperties = new List<ModelInfo>();
                _dependentDictionary[type] = dependentProperties;
            }
            else
            {
                dependentProperties = _dependentDictionary[type];
            }
            dependentProperties.Add(new ModelInfo
            {
                Property = property,
                Id = id,
                PropertyId = propertyId
            });
        }
        #endregion
    }
}