using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DocumentAPI.Infrastructure.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public ICollection<Attribute> Attributes { get; set; }
        public string NotPublicFieldName { get; set; }

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

    public static class DocumentCategories
    {
        static DocumentCategories()
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
                    new FilterType{Name = DateEqualsOperator},
                    new FilterType{Name = DateGreaterThanOperator},
                    new FilterType{Name = DateLessThanOperator},
                    new FilterType{Name = DateBetweenOperator}
                }
            };

            var textType = new Type
            {
                Name = TextTypeName,
                FilterTypes = new Collection<FilterType>
                {
                    new FilterType{Name = TextEqualsOperator},
                    new FilterType{Name = TextGreaterThanOperator},
                    new FilterType{Name = TextLessThanOperator},
                    new FilterType{Name = TextBetweenOperator}
                }
            };

            var fullTextType = new Type
            {
                Name = FullTextSearchName,
                FilterTypes = new Collection<FilterType>
                {
                    new FilterType{Name = FullTextSearchName},
                }
            };

            AttributeTypes = new Collection<Type>
            {
                numericType, dateType, textType, fullTextType
            };

            Categories = new Collection<Category>
            {
                new Category
                {
                    Id = 6,
                    Name= "HISTORICAL_COMM-CARD_CATALOG",
                    DisplayName = "Card Catalog",
                    Attributes = new Collection<Attribute>
                    {
                        new Attribute
                        {
                            FieldNumber = 1,
                            Name = "HOUSE NUMBER",
                            Type = numericType
                        },
                        new Attribute
                        {
                            FieldNumber = 2,
                            Name = "STREET DIRECTION",
                            Type = textType
                        },
                        new Attribute
                        {
                            FieldNumber = 3,
                            Name = "STREET NAME",
                            Type = textType
                        },
                        new Attribute
                        {
                            FieldNumber = 4,
                            Name = "STREET DESIGNATION",
                            Type = textType
                        },
                        new Attribute
                        {
                            FieldNumber = 5,
                            Name = "SCAN DATE",
                            Type = dateType
                        },
                        new Attribute
                        {
                            FieldNumber = 6,
                            Name = "BOX #",
                            Type = numericType
                        }
                    }
                },
                new Category
                {
                    Id = 7,
                    Name= "HISTORICAL_COMM-MEETING_MINUTES",
                    DisplayName = "Meeting Minutes",
                    NotPublicFieldName = "NOT PUBLIC",
                    Attributes = new Collection<Attribute>
                    {
                        new Attribute
                        {
                            FieldNumber = 1,
                            Name = "DOCUMENT NAME",
                            Type = textType
                        },
                        new Attribute
                        {
                            FieldNumber = 2,
                            Name = "DOCUMENT DATE",
                            Type = dateType
                        },
                        new Attribute
                        {
                            FieldNumber = 3,
                            Name = "DOCUMENT TYPE",
                            Type = textType
                        },
                        new Attribute
                        {
                            FieldNumber = 4,
                            Name = "MEETING NUMBER",
                            Type = numericType
                        },
                        new Attribute
                        {
                            FieldNumber = 5,
                            Name = "BODY",
                            Type = textType
                        },
                        new Attribute
                        {
                            FieldNumber = 6,
                            Name = "COMMENT",
                            Type = textType
                        },
                        new Attribute
                        {
                            FieldNumber = 7,
                            Name = "ARCHIVE DATE",
                            Type = dateType
                        }
                    }
                },
                new Category
                {
                    Id = 4,
                    Name= "HISTORICAL_COMM-PERMITS",
                    DisplayName = "Permits",
                    Attributes = new Collection<Attribute>
                    {
                        new Attribute
                        {
                            FieldNumber = 1,
                            Name = "PERMIT NUMBER",
                            Type = numericType
                        },
                        new Attribute
                        {
                            FieldNumber = 2,
                            Name = "HOUSE NUMBER",
                            Type = numericType
                        },
                        new Attribute
                        {
                            FieldNumber = 3,
                            Name = "STREET DIRECTION",
                            Type = textType
                        },
                        new Attribute
                        {
                            FieldNumber = 4,
                            Name = "STREET NAME",
                            Type = textType
                        },
                        new Attribute
                        {
                            FieldNumber = 5,
                            Name = "STREET DESIGNATION",
                            Type = textType
                        },
                        new Attribute
                        {
                            FieldNumber = 6,
                            Name = "LOCATION",
                            Type = textType
                        },
                        new Attribute
                        {
                            FieldNumber = 7,
                            Name = "SCAN DATE",
                            Type = dateType
                        },
                        new Attribute
                        {
                            FieldNumber = 8,
                            Name = "BOX #",
                            Type = numericType
                        }
                    }
                },
                new Category
                {
                    Id = 3,
                    Name= "HISTORICAL_COMM-POLAROIDS",
                    DisplayName = "Polaroids",
                    Attributes = new Collection<Attribute>
                    {
                        new Attribute
                        {
                            FieldNumber = 1,
                            Name = "LOCATION",
                            Type = numericType
                        },
                        new Attribute
                        {
                            FieldNumber = 2,
                            Name = "SCAN DATE",
                            Type = dateType
                        },
                        new Attribute
                        {
                            FieldNumber = 3,
                            Name = "BOX #",
                            Type = numericType
                        }
                    }
                },
                new Category
                {
                    Id = 5,
                    Name= "HISTORICAL_COMM-REGISTRY",
                    DisplayName = "Registry",
                    Attributes = new Collection<Attribute>
                    {
                        new Attribute
                        {
                            FieldNumber = 1,
                            Name = "LOCATION",
                            Type = textType
                        },
                        new Attribute
                        {
                            FieldNumber = 2,
                            Name = "SCAN DATE",
                            Type = dateType
                        },
                        new Attribute
                        {
                            FieldNumber = 3,
                            Name = "BOX #",
                            Type = numericType
                        }
                    }
                }
            };

            // Add full text search attribute to all Categories
            foreach (var category in Categories)
            {
                category.Attributes.Add(new Attribute
                {
                    FieldNumber = category.Attributes.Select(a => a.FieldNumber).Max() + 1,
                    Name = FullTextSearchName,
                    Type = fullTextType
                });

            }
        }

        public static IEnumerable<Type> AttributeTypes { get; }

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

        public static IEnumerable<Category> Categories { get; }
    }
}
