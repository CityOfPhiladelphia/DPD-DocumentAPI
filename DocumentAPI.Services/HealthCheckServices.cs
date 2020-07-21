using DocumentAPI.Infrastructure.Interfaces;
using DocumentAPI.Infrastructure.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentAPI.Services
{
    public class HealthCheckServices : IHealthCheckServices
    {
        private readonly Config _config;
        private readonly DepartmentEntities _departmentEntities;

        public HealthCheckServices(IConfiguration config)
        {
            _config = config.Load();

            _departmentEntities = new DepartmentEntities(config);
        }
        public async Task<HealthCheckResponse> CheckInternalDependencies()
        {
            var configLoaded = new HealthCheckResult() { Message = "Base configuration failed to load, check environment variables." };
            var atLeastOneEntity = new HealthCheckResult() { Message = "Department entities configuration must have at least one entity defined, check RepoConfig.json." };
            var entitiesDefinedProperly = 0;
            var allEntitiesDefinedProperly = new HealthCheckResult() { Message = "Department entities configuration is not configured properly. The following fields are required: Name, DisplayName, Id, and at least one category must be defined for Entity(s)" };
            var categoriesDefinedProperly = 0;
            var allCategoriesDefinedProperly = new HealthCheckResult() { Message = "A category within an entity configuration is not configured properly. The following fields are required: Name, DisplayName, EntityId, Id, and at least one attribute must be defined for Category(s)" };
            var attributesDefinedProperly = 0;
            var allAttributesDefinedProperly = new HealthCheckResult() { Message = "An attribute within a category configuration (within an entity) is not configured properly. The following fields are required: Name, Id, Type for the Attribute(s)" };

            var response = new HealthCheckResponse();

            // all configs should be loaded and have values
            configLoaded.Success = _config.GetType().GetProperties()
                            .Where(prop => prop.PropertyType == typeof(string))
                            .Select(prop => (string)prop.GetValue(_config))
                            .All(value => !string.IsNullOrEmpty(value));
            if (configLoaded.Success)
            {
                configLoaded.Message = "Base configuration successfully loaded.";
            }

            // department entity configuration should be loaded and have entities
            atLeastOneEntity.Success = _departmentEntities?.Entities.Any() ?? false;
            if (atLeastOneEntity.Success)
            {
                atLeastOneEntity.Message = "At least one entity has been defined.";

                var entities = _departmentEntities.Entities;
                foreach (var entity in entities)
                {
                    var entityDefiniedProperly = !string.IsNullOrEmpty(entity.Name) &&
                                !string.IsNullOrEmpty(entity.DisplayName) &&
                                entity.Id.HasValue &&
                                entity.Categories.Any();

                    if (entityDefiniedProperly)
                    {
                        entitiesDefinedProperly++;
                    }
                    else
                    {
                        allEntitiesDefinedProperly.Message += $" Entity - {entities.IndexOf(entity)}{Environment.NewLine}";
                    }
                    foreach (var category in entity.Categories)
                    {
                        var categoryDefinedProperly =
                            !string.IsNullOrEmpty(category.Name) &&
                            !string.IsNullOrEmpty(category.DisplayName) &&
                            category.EntityId.HasValue &&
                            category.Id.HasValue &&
                            category.Attributes.Any();

                        if (categoryDefinedProperly)
                        {
                            categoriesDefinedProperly++;
                        }
                        else
                        {
                            allCategoriesDefinedProperly.Message += $" Entity - {entities.IndexOf(entity)}: Category - {entity.Categories.IndexOf(category)}{Environment.NewLine}";
                        }
                        foreach (var attribute in category.Attributes)
                        {
                            var attributeDefinedProperly =
                                !string.IsNullOrEmpty(attribute.Name) &&
                                attribute.FieldNumber.HasValue &&
                                attribute.Type != null;

                            if (attributeDefinedProperly)
                            {
                                attributesDefinedProperly++;
                            }
                            else
                            {
                                allAttributesDefinedProperly.Message += $" Entity - {entities.IndexOf(entity)}: Category - {entity.Categories.IndexOf(category)}: Attribute - {category.Attributes.IndexOf(attribute)}{Environment.NewLine}";
                            }
                        }
                    }
                }
                allEntitiesDefinedProperly.Success = entitiesDefinedProperly == entities.Count();
                if (allEntitiesDefinedProperly.Success)
                {
                    allEntitiesDefinedProperly.Message = "All Entities Defined Properly.";
                }

                allCategoriesDefinedProperly.Success = categoriesDefinedProperly == entities.Select(i => i.Categories.Count()).Sum();
                if (allCategoriesDefinedProperly.Success)
                {
                    allCategoriesDefinedProperly.Message = "All Categories Defined Properly.";
                }

                allAttributesDefinedProperly.Success = attributesDefinedProperly == entities.Select(i => i.Categories.Select(c => c.Attributes.Count).Sum()).Sum();
                if (allAttributesDefinedProperly.Success)
                {
                    allAttributesDefinedProperly.Message = "All Attributes Defined Properly.";
                }
            }
            response.Results.AddRange(new[] { configLoaded, atLeastOneEntity, allEntitiesDefinedProperly, allCategoriesDefinedProperly, allAttributesDefinedProperly });

            response.Success = response.Results.All(i => i.Success);
            response.Message = response.Success ? 
                    "All configurations loaded successfully!" : 
                    "Configuration Loading Failed, due to the following reasons:" +
                        $"{Environment.NewLine}{string.Join(Environment.NewLine, response.Results.Where(i => !i.Success).Select(i => i.Message))}";
            return response;
        }

        //public async Task<HealthCheckResponse> CheckExternalDependencies()
        //{

        //}
    }
}
