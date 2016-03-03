using System;
using System.Collections.Generic;
using WebIDLCollector.IDLTypes;
using WebIDLCollector.TypeMirrorTypes;

namespace WebIDLCollector.Builders
{
    public static class BuilderHelpers
    {
        public static void AddNameLengthProto(TypeMirrorType tmType, List<Member> mem)
        {
            if (!mem.Exists(a => a.Name.Equals("length", StringComparison.OrdinalIgnoreCase)))
            {
                var length = new TypeMirrorProperty
                {
                    Name = "length",
                    Type = "unsigned long",
                    Confidence = 4,
                    HasGet = true,
                    IsConfigurable = true,
                    SpecNames = tmType.SpecNames
                };

                if (!tmType.Properties.Contains(length)) { tmType.Properties.Add(length); }
            }

            if (!mem.Exists(a => a.Name.Equals("name", StringComparison.OrdinalIgnoreCase)))
            {
                var name = new TypeMirrorProperty
                {
                    Name = "name",
                    Type = "DOMString",
                    Confidence = 4,
                    HasGet = true,
                    HasSet = true,
                    IsConfigurable = true,
                    SpecNames = tmType.SpecNames
                };

                if (!tmType.Properties.Contains(name)) { tmType.Properties.Add(name); }
            }

            if (!mem.Exists(a => a.Name.Equals("prototype", StringComparison.OrdinalIgnoreCase)))
            {
                var prototype = new TypeMirrorProperty
                {
                    Name = "prototype",
                    Type = "object",
                    Confidence = 4,
                    HasGet = true,
                    HasSet = true,
                    SpecNames = tmType.SpecNames
                };

                if (!tmType.Properties.Contains(prototype)) { tmType.Properties.Add(prototype); }
            }
        }

        public static bool AddStringifier(Member tmProperty, TypeMirrorType tmType, List<Member> mem)
        {
            if (!tmProperty.Stringifier) { return false; }
            if (string.IsNullOrWhiteSpace(tmProperty.Name))
            {
                if (mem.Exists(a => a.Name.Equals("toString", StringComparison.OrdinalIgnoreCase))) { return true; }
                var toString = new TypeMirrorProperty
                {
                    Name = "toString",
                    Type = "DOMString",
                    Confidence = 4,
                    HasGet = tmProperty.HasGet,
                    HasSet = tmProperty.HasSet,
                    IsConfigurable = true,
                    IsEnumerable = true,
                    IsWritable = true,
                    SpecNames = tmProperty.SpecNames
                };

                if (!tmType.Properties.Contains(toString)) { tmType.Properties.Add(toString); }
            }
            else
            {
                var item = new TypeMirrorProperty
                {
                    Name = tmProperty.Name,
                    Type = tmProperty.Type,
                    Confidence = 4,
                    HasGet = tmProperty.HasGet,
                    HasSet = tmProperty.HasSet,
                    IsConfigurable = true,
                    IsEnumerable = true,
                    IsWritable = true,
                    SpecNames = tmProperty.SpecNames
                };

                if (!tmType.Properties.Contains(item)) { tmType.Properties.Add(item); }
            }
            return true;
        }

        public static bool AddSetlike(Member tmProperty, TypeMirrorType tmType, List<Member> mem)
        {
            if (!tmProperty.SetLike) { return false; }
            if (!mem.Exists(a => a.Name.Equals("entries", StringComparison.OrdinalIgnoreCase)))
            {
                var entries = new TypeMirrorProperty
                {
                    Name = "entries",
                    Type = "object",
                    Confidence = 4,
                    HasGet = tmProperty.HasGet,
                    HasSet = tmProperty.HasSet,
                    IsConfigurable = true,
                    IsEnumerable = true,
                    IsWritable = true,
                    SpecNames = tmProperty.SpecNames
                };

                if (!tmType.Properties.Contains(entries)) { tmType.Properties.Add(entries); }
            }

            if (!mem.Exists(a => a.Name.Equals("forEach", StringComparison.OrdinalIgnoreCase)))
            {
                var forEach = new TypeMirrorProperty
                {
                    Name = "forEach",
                    Type = "void",
                    Confidence = 4,
                    HasGet = tmProperty.HasGet,
                    HasSet = tmProperty.HasSet,
                    IsConfigurable = true,
                    IsEnumerable = true,
                    IsWritable = true,
                    SpecNames = tmProperty.SpecNames
                };

                if (!tmType.Properties.Contains(forEach)) { tmType.Properties.Add(forEach); }
            }

            if (!mem.Exists(a => a.Name.Equals("has", StringComparison.OrdinalIgnoreCase)))
            {
                var has = new TypeMirrorProperty
                {
                    Name = "has",
                    Type = "boolean",
                    Confidence = 4,
                    HasGet = tmProperty.HasGet,
                    HasSet = tmProperty.HasSet,
                    IsConfigurable = true,
                    IsEnumerable = true,
                    IsWritable = true,
                    SpecNames = tmProperty.SpecNames
                };

                if (!tmType.Properties.Contains(has)) { tmType.Properties.Add(has); }
            }

            if (!mem.Exists(a => a.Name.Equals("keys", StringComparison.OrdinalIgnoreCase)))
            {
                var keys = new TypeMirrorProperty
                {
                    Name = "keys",
                    Type = "object",
                    Confidence = 4,
                    HasGet = tmProperty.HasGet,
                    HasSet = tmProperty.HasSet,
                    IsConfigurable = true,
                    IsEnumerable = true,
                    IsWritable = true,
                    SpecNames = tmProperty.SpecNames
                };

                if (!tmType.Properties.Contains(keys)) { tmType.Properties.Add(keys); }
            }

            if (!mem.Exists(a => a.Name.Equals("values", StringComparison.OrdinalIgnoreCase)))
            {
                var values = new TypeMirrorProperty
                {
                    Name = "values",
                    Type = "object",
                    Confidence = 4,
                    HasGet = tmProperty.HasGet,
                    HasSet = tmProperty.HasSet,
                    IsConfigurable = true,
                    IsEnumerable = true,
                    IsWritable = true,
                    SpecNames = tmProperty.SpecNames
                };

                if (!tmType.Properties.Contains(values)) { tmType.Properties.Add(values); }
            }

            if (!mem.Exists(a => a.Name.Equals("size", StringComparison.OrdinalIgnoreCase)))
            {
                var size = new TypeMirrorProperty
                {
                    Name = "size",
                    Type = "object",
                    Confidence = 4,
                    HasGet = tmProperty.HasGet,
                    IsConfigurable = true,
                    IsEnumerable = true,
                    IsWritable = true,
                    SpecNames = tmProperty.SpecNames
                };

                if (!tmType.Properties.Contains(size)) { tmType.Properties.Add(size); }
            }

            if (tmProperty.Readonly) { return true; }

            if (!mem.Exists(a => a.Name.Equals("clear", StringComparison.OrdinalIgnoreCase)))
            {
                var clear = new TypeMirrorProperty
                {
                    Name = "clear",
                    Type = "void",
                    Confidence = 4,
                    HasGet = tmProperty.HasGet,
                    HasSet = tmProperty.HasSet,
                    IsConfigurable = true,
                    IsEnumerable = true,
                    IsWritable = true,
                    SpecNames = tmProperty.SpecNames
                };

                if (!tmType.Properties.Contains(clear)) { tmType.Properties.Add(clear); }
            }

            if (!mem.Exists(a => a.Name.Equals("delete", StringComparison.OrdinalIgnoreCase)))
            {
                var delete = new TypeMirrorProperty
                {
                    Name = "delete",
                    Type = "object",
                    Confidence = 4,
                    HasGet = tmProperty.HasGet,
                    HasSet = tmProperty.HasSet,
                    IsConfigurable = true,
                    IsEnumerable = true,
                    IsWritable = true,
                    SpecNames = tmProperty.SpecNames
                };

                if (!tmType.Properties.Contains(delete)) { tmType.Properties.Add(delete); }
            }

            if (!mem.Exists(a => a.Name.Equals("add", StringComparison.OrdinalIgnoreCase)))
            {
                var add = new TypeMirrorProperty
                {
                    Name = "add",
                    Type = "object",
                    Confidence = 4,
                    HasGet = tmProperty.HasGet,
                    HasSet = tmProperty.HasSet,
                    IsConfigurable = true,
                    IsEnumerable = true,
                    IsWritable = true,
                    SpecNames = tmProperty.SpecNames
                };

                if (!tmType.Properties.Contains(add)) { tmType.Properties.Add(add); }
            }

            return true;
        }

        public static bool AddSerializer(Member tmProperty, TypeMirrorType tmType, List<Member> mem)
        {
            if (!tmProperty.Serializer) { return false; }

            if (mem.Exists(a => a.Name.Equals("toJSON", StringComparison.OrdinalIgnoreCase))) { return true; }

            var toJson = new TypeMirrorProperty
            {
                Name = "toJSON",
                Type = "object",
                Confidence = 4,
                HasGet = tmProperty.HasGet,
                HasSet = tmProperty.HasSet,
                IsConfigurable = true,
                IsEnumerable = true,
                IsWritable = true,
                SpecNames = tmProperty.SpecNames
            };

            if (!tmType.Properties.Contains(toJson)) { tmType.Properties.Add(toJson); }
            return true;
        }

        public static bool AddMaplike(Member tmProperty, TypeMirrorType tmType, List<Member> mem)
        {
            if (!tmProperty.MapLike) { return false; }

            if (!mem.Exists(a => a.Name.Equals("entries", StringComparison.OrdinalIgnoreCase)))
            {
                var entries = new TypeMirrorProperty
                {
                    Name = "entries",
                    Type = "object",
                    Confidence = 4,
                    HasGet = tmProperty.HasGet,
                    HasSet = tmProperty.HasSet,
                    IsConfigurable = true,
                    IsEnumerable = true,
                    IsWritable = true,
                    SpecNames = tmProperty.SpecNames
                };

                if (!tmType.Properties.Contains(entries)) { tmType.Properties.Add(entries); }
            }

            if (!mem.Exists(a => a.Name.Equals("forEach", StringComparison.OrdinalIgnoreCase)))
            {
                var forEach = new TypeMirrorProperty
                {
                    Name = "forEach",
                    Type = "void",
                    Confidence = 4,
                    HasGet = tmProperty.HasGet,
                    HasSet = tmProperty.HasSet,
                    IsConfigurable = true,
                    IsEnumerable = true,
                    IsWritable = true,
                    SpecNames = tmProperty.SpecNames
                };

                if (!tmType.Properties.Contains(forEach)) { tmType.Properties.Add(forEach); }
            }

            if (!mem.Exists(a => a.Name.Equals("get", StringComparison.OrdinalIgnoreCase)))
            {
                var get = new TypeMirrorProperty
                {
                    Name = "get",
                    Type = "object",
                    Confidence = 4,
                    HasGet = tmProperty.HasGet,
                    HasSet = tmProperty.HasSet,
                    IsConfigurable = true,
                    IsEnumerable = true,
                    IsWritable = true,
                    SpecNames = tmProperty.SpecNames
                };

                if (!tmType.Properties.Contains(get)) { tmType.Properties.Add(get); }
            }

            if (!mem.Exists(a => a.Name.Equals("has", StringComparison.OrdinalIgnoreCase)))
            {
                var has = new TypeMirrorProperty
                {
                    Name = "has",
                    Type = "boolean",
                    Confidence = 4,
                    HasGet = tmProperty.HasGet,
                    HasSet = tmProperty.HasSet,
                    IsConfigurable = true,
                    IsEnumerable = true,
                    IsWritable = true,
                    SpecNames = tmProperty.SpecNames
                };

                if (!tmType.Properties.Contains(has)) { tmType.Properties.Add(has); }
            }

            if (!mem.Exists(a => a.Name.Equals("keys", StringComparison.OrdinalIgnoreCase)))
            {
                var keys = new TypeMirrorProperty
                {
                    Name = "keys",
                    Type = "object",
                    Confidence = 4,
                    HasGet = tmProperty.HasGet,
                    HasSet = tmProperty.HasSet,
                    IsConfigurable = true,
                    IsEnumerable = true,
                    IsWritable = true,
                    SpecNames = tmProperty.SpecNames
                };

                if (!tmType.Properties.Contains(keys)) { tmType.Properties.Add(keys); }
            }

            if (!mem.Exists(a => a.Name.Equals("values", StringComparison.OrdinalIgnoreCase)))
            {
                var values = new TypeMirrorProperty
                {
                    Name = "values",
                    Type = "object",
                    Confidence = 4,
                    HasGet = tmProperty.HasGet,
                    HasSet = tmProperty.HasSet,
                    IsConfigurable = true,
                    IsEnumerable = true,
                    IsWritable = true,
                    SpecNames = tmProperty.SpecNames
                };

                if (!tmType.Properties.Contains(values)) { tmType.Properties.Add(values); }
            }

            if (!mem.Exists(a => a.Name.Equals("size", StringComparison.OrdinalIgnoreCase)))
            {
                var size = new TypeMirrorProperty
                {
                    Name = "size",
                    Type = "object",
                    Confidence = 4,
                    HasGet = tmProperty.HasGet,
                    IsConfigurable = true,
                    IsEnumerable = true,
                    IsWritable = true,
                    SpecNames = tmProperty.SpecNames
                };

                if (!tmType.Properties.Contains(size)) { tmType.Properties.Add(size); }
            }

            if (!mem.Exists(a => a.Name.Equals("length", StringComparison.OrdinalIgnoreCase)))
            {
                var length = new TypeMirrorProperty
                {
                    Name = "length",
                    Type = "unsigned long",
                    Confidence = 4,
                    HasGet = tmProperty.HasGet,
                    HasSet = tmProperty.HasSet,
                    IsConfigurable = true,
                    SpecNames = tmProperty.SpecNames
                };

                if (!tmType.Properties.Contains(length)) { tmType.Properties.Add(length); }
            }

            if (tmProperty.Readonly) { return true; }

            if (!mem.Exists(a => a.Name.Equals("clear", StringComparison.OrdinalIgnoreCase)))
            {
                var clear = new TypeMirrorProperty
                {
                    Name = "clear",
                    Type = "void",
                    Confidence = 4,
                    HasGet = tmProperty.HasGet,
                    HasSet = tmProperty.HasSet,
                    IsConfigurable = true,
                    IsEnumerable = true,
                    IsWritable = true,
                    SpecNames = tmProperty.SpecNames
                };

                if (!tmType.Properties.Contains(clear)) { tmType.Properties.Add(clear); }
            }

            if (!mem.Exists(a => a.Name.Equals("delete", StringComparison.OrdinalIgnoreCase)))
            {
                var delete = new TypeMirrorProperty
                {
                    Name = "delete",
                    Type = "object",
                    Confidence = 4,
                    HasGet = tmProperty.HasGet,
                    HasSet = tmProperty.HasSet,
                    IsConfigurable = true,
                    IsEnumerable = true,
                    IsWritable = true,
                    SpecNames = tmProperty.SpecNames
                };

                if (!tmType.Properties.Contains(delete)) { tmType.Properties.Add(delete); }
            }

            if (!mem.Exists(a => a.Name.Equals("set", StringComparison.OrdinalIgnoreCase)))
            {
                var set = new TypeMirrorProperty
                {
                    Name = "set",
                    Type = "object",
                    Confidence = 4,
                    HasGet = tmProperty.HasGet,
                    HasSet = tmProperty.HasSet,
                    IsConfigurable = true,
                    IsEnumerable = true,
                    IsWritable = true,
                    SpecNames = tmProperty.SpecNames
                };

                if (!tmType.Properties.Contains(set)) { tmType.Properties.Add(set); }
            }

            return true;
        }

        public static bool AddIterable(Member tmProperty, TypeMirrorType tmType, List<Member> mem)
        {
            if (!tmProperty.Iterable) { return false; }

            if (!mem.Exists(a => a.Name.Equals("entries", StringComparison.OrdinalIgnoreCase)))
            {
                var entries = new TypeMirrorProperty
                {
                    Name = "entries",
                    Type = "object",
                    Confidence = 4,
                    HasGet = tmProperty.HasGet,
                    HasSet = tmProperty.HasSet,
                    IsConfigurable = true,
                    IsEnumerable = true,
                    IsWritable = true,
                    SpecNames = tmProperty.SpecNames
                };

                if (!tmType.Properties.Contains(entries)) { tmType.Properties.Add(entries); }
            }

            if (!mem.Exists(a => a.Name.Equals("keys", StringComparison.OrdinalIgnoreCase)))
            {
                var keys = new TypeMirrorProperty
                {
                    Name = "keys",
                    Type = "object",
                    Confidence = 4,
                    HasGet = tmProperty.HasGet,
                    HasSet = tmProperty.HasSet,
                    IsConfigurable = true,
                    IsEnumerable = true,
                    IsWritable = true,
                    SpecNames = tmProperty.SpecNames
                };

                if (!tmType.Properties.Contains(keys)) { tmType.Properties.Add(keys); }
            }

            if (!mem.Exists(a => a.Name.Equals("values", StringComparison.OrdinalIgnoreCase)))
            {
                var values = new TypeMirrorProperty
                {
                    Name = "values",
                    Type = "object",
                    Confidence = 4,
                    HasGet = tmProperty.HasGet,
                    HasSet = tmProperty.HasSet,
                    IsConfigurable = true,
                    IsEnumerable = true,
                    IsWritable = true,
                    SpecNames = tmProperty.SpecNames
                };

                if (!tmType.Properties.Contains(values)) { tmType.Properties.Add(values); }
            }

            if (!mem.Exists(a => a.Name.Equals("length", StringComparison.OrdinalIgnoreCase)) && mem.Exists(a => a.Getter))
            {
                var length = new TypeMirrorProperty
                {
                    Name = "length",
                    Type = "unsigned long",
                    Confidence = 4,
                    HasGet = tmProperty.HasGet,
                    HasSet = tmProperty.HasSet,
                    IsConfigurable = true,
                    SpecNames = tmProperty.SpecNames
                };

                if (!tmType.Properties.Contains(length)) { tmType.Properties.Add(length); }
            }

            return true;
        }
    }
}