using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Umbraco.Core.Models.EntityBase;
using Umbraco.Core.Persistence.Mappers;

namespace Umbraco.Core.Models
{
    /// <summary>
    /// Defines the type of a <see cref="Property"/> object
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class PropertyType : Entity, IEquatable<PropertyType>
    {
        private string _name;
        private string _alias;
        private string _description;
        private int _dataTypeDefinitionId;
        private Lazy<int> _propertyGroupId;
        private Guid _dataTypeId;
        private DataTypeDatabaseType _dataTypeDatabaseType;
        private bool _mandatory;
        private string _helpText;
        private int _sortOrder;
        private string _validationRegExp;

        public PropertyType(IDataTypeDefinition dataTypeDefinition)
        {
            if(dataTypeDefinition.HasIdentity)
                DataTypeDefinitionId = dataTypeDefinition.Id;

            DataTypeId = dataTypeDefinition.ControlId;
            DataTypeDatabaseType = dataTypeDefinition.DatabaseType;
        }

        internal PropertyType(Guid dataTypeControlId, DataTypeDatabaseType dataTypeDatabaseType)
        {
            DataTypeId = dataTypeControlId;
            DataTypeDatabaseType = dataTypeDatabaseType;
        }

        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<PropertyType, string>(x => x.Name);
        private static readonly PropertyInfo AliasSelector = ExpressionHelper.GetPropertyInfo<PropertyType, string>(x => x.Alias);
        private static readonly PropertyInfo DescriptionSelector = ExpressionHelper.GetPropertyInfo<PropertyType, string>(x => x.Description);
        private static readonly PropertyInfo DataTypeDefinitionIdSelector = ExpressionHelper.GetPropertyInfo<PropertyType, int>(x => x.DataTypeDefinitionId);
        private static readonly PropertyInfo DataTypeControlIdSelector = ExpressionHelper.GetPropertyInfo<PropertyType, Guid>(x => x.DataTypeId);
        private static readonly PropertyInfo DataTypeDatabaseTypeSelector = ExpressionHelper.GetPropertyInfo<PropertyType, DataTypeDatabaseType>(x => x.DataTypeDatabaseType);
        private static readonly PropertyInfo MandatorySelector = ExpressionHelper.GetPropertyInfo<PropertyType, bool>(x => x.Mandatory);
        private static readonly PropertyInfo HelpTextSelector = ExpressionHelper.GetPropertyInfo<PropertyType, string>(x => x.HelpText);
        private static readonly PropertyInfo SortOrderSelector = ExpressionHelper.GetPropertyInfo<PropertyType, int>(x => x.SortOrder);
        private static readonly PropertyInfo ValidationRegExpSelector = ExpressionHelper.GetPropertyInfo<PropertyType, string>(x => x.ValidationRegExp);
        private static readonly PropertyInfo PropertyGroupIdSelector = ExpressionHelper.GetPropertyInfo<PropertyType, Lazy<int>>(x => x.PropertyGroupId);

        /// <summary>
        /// Gets of Sets the Name of the PropertyType
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return _name; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _name = value;
                    return _name;
                }, _name, NameSelector);
            }
        }

        /// <summary>
        /// Gets of Sets the Alias of the PropertyType
        /// </summary>
        [DataMember]
        public string Alias
        {
            get { return _alias; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _alias = value;
                    return _alias;
                }, _alias, AliasSelector);
            }
        }

        /// <summary>
        /// Gets of Sets the Description for the PropertyType
        /// </summary>
        [DataMember]
        public string Description
        {
            get { return _description; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _description = value;
                    return _description;
                }, _description, DescriptionSelector);
            }
        }

        /// <summary>
        /// Gets of Sets the Id of the DataType (Definition), which the PropertyType is "wrapping"
        /// </summary>
        /// <remarks>This is actually the Id of the <see cref="IDataTypeDefinition"/></remarks>
        [DataMember]
        public int DataTypeDefinitionId
        {
            get { return _dataTypeDefinitionId; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _dataTypeDefinitionId = value;
                    return _dataTypeDefinitionId;
                }, _dataTypeDefinitionId, DataTypeDefinitionIdSelector);
            }
        }

        /// <summary>
        /// Gets of Sets the Id of the DataType control
        /// </summary>
        /// <remarks>This is the Id of the actual DataType control</remarks>
        [DataMember]
        public Guid DataTypeId
        {
            get { return _dataTypeId; }
            internal set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _dataTypeId = value;
                    return _dataTypeId;
                }, _dataTypeId, DataTypeControlIdSelector);
            }
        }

        /// <summary>
        /// Gets or Sets the DatabaseType for which the DataType's value is saved as
        /// </summary>
        [DataMember]
        internal DataTypeDatabaseType DataTypeDatabaseType
        {
            get { return _dataTypeDatabaseType; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _dataTypeDatabaseType = value;
                    return _dataTypeDatabaseType;
                }, _dataTypeDatabaseType, DataTypeDatabaseTypeSelector);
            }
        }

        /// <summary>
        /// Gets or Sets the PropertyGroup's Id for which this PropertyType belongs
        /// </summary>
        [DataMember]
        internal Lazy<int> PropertyGroupId
        {
            get { return _propertyGroupId; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _propertyGroupId = value;
                    return _propertyGroupId;
                }, _propertyGroupId, PropertyGroupIdSelector);
            }
        }

        /// <summary>
        /// Gets of Sets the Boolean indicating whether a value for this PropertyType is required
        /// </summary>
        [DataMember]
        public bool Mandatory
        {
            get { return _mandatory; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _mandatory = value;
                    return _mandatory;
                }, _mandatory, MandatorySelector);
            }
        }

        /// <summary>
        /// Gets of Sets the Help text for the current PropertyType
        /// </summary>
        [DataMember]
        [Obsolete("Not used anywhere in the UI")]
        public string HelpText
        {
            get { return _helpText; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _helpText = value;
                    return _helpText;
                }, _helpText, HelpTextSelector);
            }
        }

        /// <summary>
        /// Gets of Sets the Sort order of the PropertyType, which is used for sorting within a group
        /// </summary>
        [DataMember]
        public int SortOrder
        {
            get { return _sortOrder; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _sortOrder = value;
                    return _sortOrder;
                }, _sortOrder, SortOrderSelector);
            }
        }

        /// <summary>
        /// Gets or Sets the RegEx for validation of legacy DataTypes
        /// </summary>
        [DataMember]
        public string ValidationRegExp
        {
            get { return _validationRegExp; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _validationRegExp = value;
                    return _validationRegExp;
                }, _validationRegExp, ValidationRegExpSelector);
            }
        }

        /// <summary>
        /// Create a new Property object from a "raw" database value.
        /// </summary>
        /// <remarks>Can be used for the "old" values where no serialization type exists</remarks>
        /// <param name="value"></param>
        /// <param name="version"> </param>
        /// <param name="id"> </param>
        /// <returns></returns>
        internal Property CreatePropertyFromRawValue(object value, Guid version, int id)
        {
            return new Property(id, version, this, value);
        }

        /// <summary>
        /// Create a new Property object from a "raw" database value.
        /// In some cases the value will need to be deserialized.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="serializationType"> </param>
        /// <returns></returns>
        internal Property CreatePropertyFromRawValue(object value, string serializationType)
        {
            //The value from the db needs to be deserialized and then added to the property
            //if its not a simple type (Integer, Date, Nvarchar, Ntext)
            /*if (DataTypeDatabaseType == DataTypeDatabaseType.Object)
            {
                Type type = Type.GetType(serializationType);
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(value.ToString()));
                var objValue = _service.FromStream(stream, type);
                return new Property(this, objValue);
            }*/

            return new Property(this, value);
        }

        /// <summary>
        /// Create a new Property object that conforms to the Type of the DataType
        /// and can be validated according to DataType validation / Mandatory-check.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Property CreatePropertyFromValue(object value)
        {
            //Note that validation will occur when setting the value on the Property
            return new Property(this, value);
        }

        /// <summary>
        /// Validates the Value from a Property according to its type
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True if valid, otherwise false</returns>
        public bool IsPropertyTypeValid(object value)
        {
            //Can't validate null values, so just allow it to pass the current validation
            if (value == null)
                return true;

            //Check type if the type of the value match the type from the DataType/PropertyEditor
            Type type = value.GetType();

            //TODO Add PropertyEditor Type validation when its relevant to introduce
            /*bool isEditorModel = value is IEditorModel;
            if (isEditorModel && DataTypeControlId != Guid.Empty)
            {
                //Find PropertyEditor by Id
                var propertyEditor = PropertyEditorResolver.Current.GetById(DataTypeControlId);

                if (propertyEditor == null)
                    return false;//Throw exception instead?

                //Get the generic parameter of the PropertyEditor and check it against the type of the passed in (object) value
                Type argument = propertyEditor.GetType().BaseType.GetGenericArguments()[0];
                return argument == type;
            }*/

            if (DataTypeId != Guid.Empty)
            {
                //Find DataType by Id
                //IDataType dataType = DataTypesResolver.Current.GetById(DataTypeControlId);
                //Check if dataType is null (meaning that the ControlId is valid) ?
                //Possibly cast to BaseDataType and get the DbType from there (which might not be possible because it lives in umbraco.cms.businesslogic.datatype) ?

                //Simple validation using the DatabaseType from the DataTypeDefinition and Type of the passed in value
                if (DataTypeDatabaseType == DataTypeDatabaseType.Integer && type == typeof(int))
                    return true;

                if (DataTypeDatabaseType == DataTypeDatabaseType.Date && type == typeof(DateTime))
                    return true;

                if (DataTypeDatabaseType == DataTypeDatabaseType.Nvarchar && type == typeof(string))
                    return true;

                if (DataTypeDatabaseType == DataTypeDatabaseType.Ntext && type == typeof(string))
                    return true;
            }

            //Fallback for simple value types when no Control Id or Database Type is set
            if (type.IsPrimitive || value is string)
                return true;

            return false;
        }

        /// <summary>
        /// Validates the Value from a Property according to the validation settings
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True if valid, otherwise false</returns>
        public bool IsPropertyValueValid(object value)
        {
            //If the Property is mandatory and value is null or empty, return false as the validation failed
            if (Mandatory && (value == null || string.IsNullOrEmpty(value.ToString())))
                return false;

            //Check against Regular Expression for Legacy DataTypes - Validation exists and value is not null:
            if(string.IsNullOrEmpty(ValidationRegExp) == false && (value != null && string.IsNullOrEmpty(value.ToString()) == false))
            {
                var regexPattern = new Regex(ValidationRegExp);
                return regexPattern.IsMatch(value.ToString());
            }

            //TODO Add PropertyEditor validation when its relevant to introduce
            /*if (value is IEditorModel && DataTypeControlId != Guid.Empty)
            {
                //Find PropertyEditor by Id
                var propertyEditor = PropertyEditorResolver.Current.GetById(DataTypeControlId);

                //TODO Get the validation from the PropertyEditor if a validation attribute exists
                //Will probably need to reflect the PropertyEditor in order to apply the validation
            }*/

            return true;
        }

        internal PropertyType Clone()
        {
            var clone = (PropertyType) this.MemberwiseClone();
            clone.ResetIdentity();
            clone.ResetDirtyProperties(false);
            return clone;
        }

        public bool Equals(PropertyType other)
        {
            //Check whether the compared object is null. 
            if (Object.ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data. 
            if (Object.ReferenceEquals(this, other)) return true;

            //Check whether the PropertyType's properties are equal. 
            return Alias.Equals(other.Alias) && Name.Equals(other.Name);
        }

        public override int GetHashCode()
        {
            //Get hash code for the Name field if it is not null. 
            int hashName = Name == null ? 0 : Name.GetHashCode();

            //Get hash code for the Alias field. 
            int hashAlias = Alias.GetHashCode();

            //Calculate the hash code for the product. 
            return hashName ^ hashAlias;
        }
    }
}