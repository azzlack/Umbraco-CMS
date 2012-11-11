﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Umbraco.Core.Models
{
    /// <summary>
    /// Represents the contnet type that a <see cref="Content"/> object is based on
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class ContentType : ContentTypeCompositionBase, IContentType
    {
        private int _defaultTemplate;
        private IEnumerable<ITemplate> _allowedTemplates;

        public ContentType(int parentId) : base(parentId)
        {
            _allowedTemplates = new List<ITemplate>();
        }

        private static readonly PropertyInfo DefaultTemplateSelector = ExpressionHelper.GetPropertyInfo<ContentType, int>(x => x.DefaultTemplateId);
        private static readonly PropertyInfo AllowedTemplatesSelector = ExpressionHelper.GetPropertyInfo<ContentType, IEnumerable<ITemplate>>(x => x.AllowedTemplates);
        
        /// <summary>
        /// Gets or sets the alias of the default Template.
        /// </summary>
        [DataMember]
        public ITemplate DefaultTemplate
        {
            get { return AllowedTemplates.FirstOrDefault(x => x.Id == DefaultTemplateId); }
        }

        /// <summary>
        /// Internal property to store the Id of the default template
        /// </summary>
        [DataMember]
        internal int DefaultTemplateId
        {
            get { return _defaultTemplate; }
            set
            {
                _defaultTemplate = value;
                OnPropertyChanged(DefaultTemplateSelector);
            }
        }

        /// <summary>
        /// Gets or Sets a list of Templates which are allowed for the ContentType
        /// </summary>
        [DataMember]
        public IEnumerable<ITemplate> AllowedTemplates
        {
            get { return _allowedTemplates; }
            set
            {
                _allowedTemplates = value;
                OnPropertyChanged(AllowedTemplatesSelector);
            }
        }

        /// <summary>
        /// Indicates whether a specific property on the current <see cref="IContent"/> entity is dirty.
        /// </summary>
        /// <param name="propertyName">Name of the property to check</param>
        /// <returns>True if Property is dirty, otherwise False</returns>
        public override bool IsPropertyDirty(string propertyName)
        {
            bool existsInEntity = base.IsPropertyDirty(propertyName);

            bool anyDirtyGroups = PropertyGroups.Any(x => x.IsPropertyDirty(propertyName));
            bool anyDirtyTypes = PropertyTypes.Any(x => x.IsPropertyDirty(propertyName));

            return existsInEntity || anyDirtyGroups || anyDirtyTypes;
        }

        /// <summary>
        /// Indicates whether the current entity is dirty.
        /// </summary>
        /// <returns>True if entity is dirty, otherwise False</returns>
        public override bool IsDirty()
        {
            bool dirtyEntity = base.IsDirty();

            bool dirtyGroups = PropertyGroups.Any(x => x.IsDirty());
            bool dirtyTypes = PropertyTypes.Any(x => x.IsDirty());

            return dirtyEntity || dirtyGroups || dirtyTypes;
        }

        /// <summary>
        /// Resets dirty properties by clearing the dictionary used to track changes.
        /// </summary>
        /// <remarks>
        /// Please note that resetting the dirty properties could potentially
        /// obstruct the saving of a new or updated entity.
        /// </remarks>
        public override void ResetDirtyProperties()
        {
            base.ResetDirtyProperties();

            foreach (var propertyGroup in PropertyGroups)
            {
                propertyGroup.ResetDirtyProperties();
                foreach (var propertyType in propertyGroup.PropertyTypes)
                {
                    propertyType.ResetDirtyProperties();
                }
            }
        }

        /// <summary>
        /// Method to call when Entity is being saved
        /// </summary>
        /// <remarks>Created date is set and a Unique key is assigned</remarks>
        internal override void AddingEntity()
        {
            base.AddingEntity();
            Key = Guid.NewGuid();
        }

        /// <summary>
        /// Method to call when Entity is being updated
        /// </summary>
        /// <remarks>Modified Date is set and a new Version guid is set</remarks>
        internal override void UpdatingEntity()
        {
            base.UpdatingEntity();
        }
    }
}