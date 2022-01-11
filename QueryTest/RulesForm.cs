using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QueryTest {
	public partial class RulesForm : Form {
		public RulesForm(Dictionary<string, string> rules) {
			InitializeComponent();

			propGrid.SelectedObject = new PropAdapter(rules);
		}

		private void RulesForm_Load(object sender, EventArgs e) {

		}
	}

	internal class PropAdapter : ICustomTypeDescriptor {
		private Dictionary<string, string> rules;

		public PropAdapter(Dictionary<string, string> rules) {
			this.rules = rules ?? throw new ArgumentNullException(nameof(rules));
		}

		public string GetClassName() {
			return TypeDescriptor.GetClassName(this, true);
		}

		public string GetComponentName() {
			return TypeDescriptor.GetComponentName(this, true);
		}

		public EventDescriptor GetDefaultEvent() {
			return TypeDescriptor.GetDefaultEvent(this, true);
		}
        public EventDescriptorCollection GetEvents(Attribute[] attributes) {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents() {
            return TypeDescriptor.GetEvents(this, true);
        }

        public TypeConverter GetConverter() {
            return TypeDescriptor.GetConverter(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd) {
            return rules;
        }

        public AttributeCollection GetAttributes() {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public object GetEditor(Type editorBaseType) {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public PropertyDescriptor GetDefaultProperty() {
            return null;
        }

        PropertyDescriptorCollection
            System.ComponentModel.ICustomTypeDescriptor.GetProperties() {
            return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) {
            var properties = new List<PropertyDescriptor>();
            foreach(var kv in rules) {
				List<Attribute> atts = new List<Attribute>();

				if(kv.Key.StartsWith("tf_passtime")) {
					atts.Add(new CategoryAttribute("Passtime"));
				} else if(kv.Key.StartsWith("tf_")) {
					atts.Add(new CategoryAttribute("Team Fortress 2"));
				} else if(kv.Key.StartsWith("sm_")) {
					atts.Add(new CategoryAttribute("Source Mod"));
				} else if(kv.Key.StartsWith("tv_")) {
					atts.Add(new CategoryAttribute("Source TV"));
				}

				if(kv.Key=="sv_tags") {
					properties.Add(new TagsPropertyDescriptor(kv.Key, atts.ToArray()));
					continue;
				}


				properties.Add(new RulePropertyDescriptor(kv.Key,atts.ToArray()));
            }

            return new PropertyDescriptorCollection(properties.ToArray());
        }
    }

	class RulePropertyDescriptor : PropertyDescriptor {
		protected string key;

		public RulePropertyDescriptor(string key, Attribute[] atts) : base(key, atts) {
			this.key = key;
		}

		public override Type ComponentType => typeof(Dictionary<string,string>);

		public override bool IsReadOnly => true;

		public override Type PropertyType => typeof(string);

		public override bool CanResetValue(object component) => false;

		public override object GetValue(object component) {
			var dict = (Dictionary<string, string>)component;
			return dict[key];
		}

		public override void ResetValue(object component) {
			throw new NotImplementedException();
		}

		public override void SetValue(object component, object value) {
			var dict = (Dictionary<string, string>)component;
			dict[key] = value.ToString();
		}

		public override bool ShouldSerializeValue(object component) => false;
	}

	class TagsPropertyDescriptor : RulePropertyDescriptor {
		public TagsPropertyDescriptor(string key, Attribute[] atts) : base(key, atts) {
		}

		public override Type PropertyType => typeof(string[]);

		public override object GetValue(object component) {
			var dict = (Dictionary<string, string>)component;
			return dict[key].Split(',');
		}

		public override void SetValue(object component, object value) {
			var dict = (Dictionary<string, string>)component;
			dict[key] = String.Join(",",(string[])value);
		}
	}
}
