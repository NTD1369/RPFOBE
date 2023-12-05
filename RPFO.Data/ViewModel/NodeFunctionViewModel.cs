using RPFO.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPFO.Data.ViewModel
{
    public class NodeFunctionViewModel //: MFunction
    {
        //List<> Data { get; set; }
        public NodeFunctionViewModel()
        {
            Children = new List<NodeFunctionViewModel>();
        }
       public  MFunction Data { get; set; }
        public List<NodeFunctionViewModel> Children { get; set; }
    }

    public class NodeViewModel //: MFunction
    {
        //List<> Data { get; set; }
        public NodeViewModel()
        {
            Children = new List<dynamic>();
        }
        public dynamic Data { get; set; }
        public List<dynamic> Children { get; set; }
    }
}
