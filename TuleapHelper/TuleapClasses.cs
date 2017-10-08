using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuleapHelper
{
    public class TuleapClasses
    {

        public class Track
        {
            public int trackerId { get; set; }
            public struct Fild
            {
                int FieldId;
                string valueparamName;
                object valueByDefault;
            }
            public List<Fild> RequiredFields { get; set; }
        }

        public class UserInfo
        {
            public string user_id;
            public string token;
            public string uri;
        }

        public class Resource
        {
            public string type { get; set; }
            public string uri { get; set; }
        }

        public class Project
        {
            public int id { get; set; }
            public string uri { get; set; }
            public string label { get; set; }
            public string shortname { get; set; }
            public List<Resource> resources { get; set; }
            public List<object> additional_informations { get; set; }
            public List<Tracker3> trackers { get; set; }
            //public List<string> requiredFields { get; set; }
            //public int TaskTracker
        }


        public class Tracker
        {
            public int id { get; set; }
            public string uri { get; set; }
        }

        //public class Artifact
        //{
        //    public int id { get; set; }
        //    public string uri { get; set; }
        //    public Tracker tracker { get; set; }
        //    public Project project { get; set; }
        //    public int submitted_by { get; set; }
        //    public string submitted_on { get; set; }
        //    public string html_url { get; set; }
        //    public string changesets_uri { get; set; }
        //    public object values { get; set; }
        //    public object values_by_field { get; set; }
        //    public string last_modified_date { get; set; }
        //}


        public enum ArtifactTypes
        { Bug, Patch, Task }

        public class Artifact
        {
            public int id { get; set; }
            public string uri { get; set; }
            public Tracker3 tracker { get; set; }  // todo !!!!!!! here was Tracker (not Tracker3)
            public Project project { get; set; }
            public int submitted_by { get; set; }
            public string submitted_on { get; set; }
            public string html_url { get; set; }
            public string changesets_uri { get; set; }
            public List<Value> values { get; set; }
            public object values_by_field { get; set; }
            public string last_modified_date { get; set; }
            public ArtifactTypes ArtifactType { get; set; }
        }

        public class Color
        {
            public int r { get; set; }
            public int g { get; set; }
            public int b { get; set; }
        }

        public class User
        {
            public string email { get; set; }
            public string status { get; set; }
            public object id { get; set; }
            public string uri { get; set; }
            public string user_url { get; set; }
            public string real_name { get; set; }
            public string display_name { get; set; }
            public string username { get; set; }
            public string ldap_id { get; set; }
            public string avatar_url { get; set; }
            public bool is_anonymous { get; set; }
            public string label { get; set; }
            public Color color { get; set; }
        }

        public class Tracker2
        {
            public int id { get; set; }
            public string uri { get; set; }
            public string label { get; set; }
        }

        //[JsonArrayAttribute]
        //[JsonObjectAttribute]
        public class FieldValues     //
        {
            public int id { get; set; }
            public string label { get; set; }
            //public Boolean is_used_by_default { get; set; }
        }
        public class Field
        {
            public int field_id { get; set; }
            public string label { get; set; }
            public string name { get; set; }
            public string type { get; set; }  // пока не используется
            public object values { get; set; }
            public FieldValues[] FValues { get; set; } 
        }
        public class Tracker3
        {
            public int id { get; set; }
            public string label { get; set; }
            public string description { get; set; }
            public string item_name { get; set; }
            public List<Field> fields { get; set; }

            public List<Form1.TrackerField> RequiredFields { get; set; }
        }


        public class ReverseLink
        {
            public int id { get; set; }
            public string uri { get; set; }
            public Tracker2 tracker { get; set; }
        }

        public class FileDescription
        {
            public int id { get; set; }
            public int submitted_by { get; set; }
            public string description { get; set; }
            public string name { get; set; }
            public int size { get; set; }
            public string type { get; set; }
            public string html_url { get; set; }
            public string html_preview_url { get; set; }
            public string uri { get; set; }
        }

        public class Value
        {
            public int field_id { get; set; }
            public string type { get; set; }
            public string label { get; set; }
            public object value { get; set; }
            public List<User> values { get; set; }
            public List<int?> bind_value_ids { get; set; }
            public List<object> links { get; set; }
            public List<ReverseLink> reverse_links { get; set; }
            public List<object> granted_groups { get; set; }
            public List<object> granted_groups_details { get; set; }
            public List<FileDescription> file_descriptions { get; set; }
            public string format { get; set; }
        }

    }
}
