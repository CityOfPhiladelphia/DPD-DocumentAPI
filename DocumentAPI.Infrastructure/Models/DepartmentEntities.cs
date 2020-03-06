using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;

namespace DocumentAPI.Infrastructure.Models
{
    public class DepartmentEntities
    {
        private IConfiguration _config;

        public DepartmentEntities(IConfiguration config)
        {
            _config = config;

            var attributeTypes = GetAttributeTypes();

            var entities = _config.GetSection("Entities").GetChildren().ToList().Select(e =>
            new Entity
            {
                Id = int.Parse(e["id"]),
                Name = e["name"],
                DisplayName = e["displayname"],
                Categories = e.GetSection("categories").GetChildren()
                                        .Select(i => new Category
                                        {
                                            Id = int.Parse(i["id"]),
                                            Name = i["name"],
                                            DisplayName = i["displayName"],
                                            EntityId = int.Parse(e["id"]),
                                            NotPublicFieldName = i["notPublicFieldName"],
                                            Attributes = i.GetSection("attributes").GetChildren()
                                                .Select(x => new Attribute
                                                {
                                                    FieldNumber = int.Parse(x["fieldNumber"]),
                                                    Name = x["name"],
                                                    Type = attributeTypes.SingleOrDefault(a => a.Name == x["type"])
                                                }).ToList()
                                        })
            });

            // Add full text search attribute to all Categories for all Entities
            foreach (var entity in entities)
            {
                foreach (var category in entity.Categories)
                {
                    category.Attributes.Add(new Attribute
                    {
                        FieldNumber = category.Attributes.Select(a => a.FieldNumber).Max() + 1,
                        Name = FullTextSearchName,
                        Type = attributeTypes.SingleOrDefault(i => i.Name == FullTextSearchName)
                    });
                }
            }
            Entities = entities.ToList();
        }

        private IEnumerable<Type> GetAttributeTypes()
        {
            var numericType = new Type
            {
                Name = NumericTypeName,
                FilterTypes = new Collection<FilterType>
                     {
                         new FilterType {Name = NumericEqualsOperator},
                         new FilterType {Name = NumericGreaterThanOperator},
                         new FilterType {Name = NumericLessThanOperator},
                         new FilterType {Name = NumericBetweenOperator}
                     }
            };

            var dateType = new Type
            {
                Name = DateTypeName,
                FilterTypes = new Collection<FilterType>
                     {
                         new FilterType {Name = DateEqualsOperator},
                         new FilterType {Name = DateGreaterThanOperator},
                         new FilterType {Name = DateLessThanOperator},
                         new FilterType {Name = DateBetweenOperator}
                     }
            };

            var textType = new Type
            {
                Name = TextTypeName,
                FilterTypes = new Collection<FilterType>
                     {
                         new FilterType {Name = TextEqualsOperator},
                         new FilterType {Name = TextGreaterThanOperator},
                         new FilterType {Name = TextLessThanOperator},
                         new FilterType {Name = TextBetweenOperator}
                     }
            };

            var fullTextType = new Type
            {
                Name = FullTextSearchName,
                FilterTypes = new Collection<FilterType>
                     {
                         new FilterType {Name = FullTextSearchName},
                     }
            };

            return new Collection<Type> { numericType, dateType, textType, fullTextType };
        }

        public List<Entity> Entities { get; set; }

        public const string NumericTypeName = "NUMERIC";

        public const string NumericEqualsOperator = "EQUALS";

        public const string NumericGreaterThanOperator = "GREATER_THAN";

        public const string NumericLessThanOperator = "LESS_THAN";

        public const string NumericBetweenOperator = "BETWEEN";

        public const string DateEqualsOperator = "EQUALS";

        public const string DateGreaterThanOperator = "AFTER";

        public const string DateLessThanOperator = "BEFORE";

        public const string DateBetweenOperator = "BETWEEN";

        public const string TextEqualsOperator = "EQUALS";

        public const string TextGreaterThanOperator = "STARTS_WITH";

        public const string TextLessThanOperator = "ENDS_WITH";

        public const string TextBetweenOperator = "CONTAINS";

        public const string DateTypeName = "DATE";

        public const string TextTypeName = "TEXT";

        public const string FullTextSearchName = "FULL_TEXT";
    }

    public class Entity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public IEnumerable<Category> Categories { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public ICollection<Attribute> Attributes { get; set; }
        public string NotPublicFieldName { get; set; }

        public int EntityId { get; set; }
    }

    public class Attribute
    {
        public int FieldNumber { get; set; }

        public string Name { get; set; }

        public string FilterValue1 { get; set; }

        public string FilterValue2 { get; set; }

        public Type Type { get; set; }

        public FilterType SelectedFilterType { get; set; }
    }

    public class Type
    {
        public string Name { get; set; }
        public IEnumerable<FilterType> FilterTypes { get; set; }
    }


    public class FilterType
    {
        public string Name { get; set; }
    }
}
