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
				properties.Add(descriptorForRule(kv.Key));
			}

            return new PropertyDescriptorCollection(properties.ToArray());
        }

		private static PropertyDescriptor descriptorForRule(string key) {
			List<Attribute> atts = new List<Attribute>();
			if(key.EndsWith("_version")) {
				atts.Add(new CategoryAttribute("Version"));
			} else if(key.StartsWith("tf_passtime")) {
				atts.Add(new CategoryAttribute("Passtime"));
			} else if(key.StartsWith("tf_arena_")) {
				atts.Add(new CategoryAttribute("Arena"));
			} else if(key.StartsWith("mp_tournament")) {
				atts.Add(new CategoryAttribute("Tournament"));
			} else if(key.StartsWith("tf_")) {
				atts.Add(new CategoryAttribute("Team Fortress 2"));
			} else if(key.StartsWith("sm_")) {
				atts.Add(new CategoryAttribute("Source Mod"));
			} else if(key.StartsWith("tv_")) {
				atts.Add(new CategoryAttribute("Source TV"));
			}

			if(key == "sv_tags") {
				return new ListPropertyDescriptor(key, atts.ToArray(), ',');
			} else if(key == "mp_teamlist") {
				return new ListPropertyDescriptor(key, atts.ToArray(), ';');
			}

			switch(key) {
				case "sv_alltalk":
				case "sv_cheats":
				case "sv_password":
				case "sv_pausable":
				case "sv_voiceenable":
				case "tf_arena_first_blood":
				case "tf_arena_force_class":
				case "tf_arena_use_queue":
				case "tv_enable":
				case "tv_password":
				case "tf_allow_player_name_change":
				case "tf_allow_player_use":
				case "tf_gravetalk":
				case "tf_halloween_allow_truce_during_boss_event":
				case "tf_medieval":
				case "tf_medieval_autorp":
				case "tf_powerup_mode":
				case "tf_server_identity_disable_quickplay":
				case "tf_spells_enabled":
				case "tf_use_fixed_weaponspreads":
				case "tf_weapon_criticals":
				case "tf_weapon_criticals_melee":
					return new BoolPropertyDescriptor(key, atts.ToArray());
			}

			return new StringPropertyDescriptor(key, atts.ToArray());
		}
	}

	abstract class RulePropertyDescriptor : PropertyDescriptor {
		protected string key;

		public RulePropertyDescriptor(string key, Attribute[] atts) : base(key, atts) {
			this.key = key;
		}

		public override Type ComponentType => typeof(Dictionary<string,string>);

		public override bool IsReadOnly => true;

		public override bool CanResetValue(object component) => false;

		public override void ResetValue(object component) {
			throw new NotImplementedException();
		}

		public override void SetValue(object component, object value) {
			var dict = (Dictionary<string, string>)component;
			dict[key] = value.ToString();
		}

		public override bool ShouldSerializeValue(object component) => false;
	}

	class StringPropertyDescriptor : RulePropertyDescriptor {
		public StringPropertyDescriptor(string key, Attribute[] atts) : base(key, atts) {
		}

		public override Type PropertyType => typeof(string);

		public override object GetValue(object component) {
			var dict = (Dictionary<string, string>)component;
			return dict[key];
		}
	}

	class FloatPropertyDescriptor : RulePropertyDescriptor {
		public FloatPropertyDescriptor(string key, Attribute[] atts) : base(key, atts) {
		}

		public override Type PropertyType => typeof(float);

		public override object GetValue(object component) {
			var dict = (Dictionary<string, string>)component;
			return float.Parse(dict[key]);
		}
	}

	class IntPropertyDescriptor : RulePropertyDescriptor {
		public IntPropertyDescriptor(string key, Attribute[] atts) : base(key, atts) {
		}

		public override Type PropertyType => typeof(int);

		public override object GetValue(object component) {
			var dict = (Dictionary<string, string>)component;
			return int.Parse(dict[key]);
		}
	}

	class BoolPropertyDescriptor : RulePropertyDescriptor {
		public BoolPropertyDescriptor(string key, Attribute[] atts) : base(key, atts) {
		}

		public override Type PropertyType => typeof(bool);

		public override object GetValue(object component) {
			var dict = (Dictionary<string, string>)component;
			return int.Parse(dict[key])!=0;
		}
	}

	class ListPropertyDescriptor : RulePropertyDescriptor {
		private char separator;
		public ListPropertyDescriptor(string key, Attribute[] atts, char separator) : base(key, atts) {
			this.separator = separator;
		}

		public override Type PropertyType => typeof(string[]);

		public override object GetValue(object component) {
			var dict = (Dictionary<string, string>)component;
			return dict[key].Split(separator);
		}

		public override void SetValue(object component, object value) {
			var dict = (Dictionary<string, string>)component;
			dict[key] = String.Join(separator.ToString(), (string[])value);
		}
	}
}
