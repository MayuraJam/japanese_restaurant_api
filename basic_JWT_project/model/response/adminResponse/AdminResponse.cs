using japanese_resturant_project.model.DatabaseModel;
using japanese_resturant_project.services;

namespace japanese_resturant_project.model.response.adminResponse
{
    public class AdminResponse : Message
    {
        public List<Option_tb_> optionList {  get; set; }
        public Option_tb_ optionitem { get; set; }
        public Menu_tb menuitem { get; set; }
        public List<Menu_tb> menuList { get; set; }

    }
}
