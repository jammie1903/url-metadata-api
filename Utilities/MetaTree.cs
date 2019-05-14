using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace UrlMetadata.Utilities
{
    public class MetaTreeItemConverter : JsonConverter<MetaTreeItem>
    {
        public override MetaTreeItem ReadJson(JsonReader reader, Type objectType, MetaTreeItem existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, MetaTreeItem value, JsonSerializer serializer)
        {
            if (value.HasChildValues)
            {
               serializer.Serialize(writer, value.Values);
            }
            else if (value.Value != null)
            {
                serializer.Serialize(writer, value.Value);
            }
            else
            {
                writer.WriteNull();
            }
        }
    }

    [JsonConverter(typeof(MetaTreeItemConverter))]
    public class MetaTreeItem
    {
        public object Value { get; internal set; }
        private readonly IDictionary<string, IList<MetaTreeItem>> _items = new Dictionary<string, IList<MetaTreeItem>>();

        public bool HasChildValues => _items.Count > 0;
        public IDictionary<string, object> Values {
            get
            {
                var val = _items.ToDictionary(entry => entry.Key, entry => GetValue(entry.Value));
                if (Value != null) val["_value"] = new MetaTreeItem(Value);
                return val;
            }
        }


        private static object GetValue(ICollection<MetaTreeItem> value)
        {
            switch (value.Count)
            {
                case 0:
                    return null;
                case 1:
                    return value.First();
                default:
                    return value;
            }

        }

        public MetaTreeItem(object value = null)
        {
            Value = value;
        }

        internal MetaTreeItem Get(string key)
        {
            if (!_items.ContainsKey(key))
            {
                _items[key] = new List<MetaTreeItem> {new MetaTreeItem()};
            }

            return _items[key].Last();
        }

        internal void Set(string key, object value)
        {
            if (Get(key).Value != null)
            {
                _items[key].Add(new MetaTreeItem());
            }

            Get(key).Value = value;
        }
    }

    public class MetaTree
    {
        public MetaTreeItem Root { get; } = new MetaTreeItem();

        public void Add(string key, object value)
        {
            var segments = new Queue<string>(key.Split(':'));
            var item = Root;

            while (segments.Count > 1)
            {
                item = item.Get(segments.Dequeue());
            }

            item.Set(segments.Dequeue(), value);
        }
    }
}
