﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KHGraphDB.Structure;
using System.IO;

namespace KHGraphDB.Helper
{
    public class GraphWriter
    {
        /// <summary>
        /// 获取Writer所绑定的图
        /// </summary>
        public Graph Graph { get { return _graph ; } }
        private Graph _graph;

        /// <summary>
        /// 获取和设置数据库存储的位置
        /// </summary>
        public String Path { get; set; }

        public GraphWriter(Graph g)
        {
            _graph = g;
            Path = "Graph_lyf";
        }

        public bool Write()
        {
            using (StreamWriter sr = new StreamWriter(Path + "_Type.gdbt"))
            {
                WriteTypes(sr);
            }

            using (StreamWriter sr = new StreamWriter(Path + "_Vertex.gdbt"))
            {
                WriteVertexs(sr);
            }
            using (StreamWriter sr = new StreamWriter(Path + "_Edge.gdbt"))
            {
                WriteEdges(sr);
            }
            return true;
        }

        public void WriteTypes(StreamWriter sr)
        {
            string str = "";
            foreach (var t in Graph.Types)
            {
                str = t.KHID + " " + t.Attributes.Count.ToString() + " ";
                foreach (var key in t.Attributes.Keys)
                {
                    str += key + " " + ((t[key] == null) ? "*" : t[key].ToString()) + " ";
                }
                str += t.Vertices.Count().ToString() + " ";
                foreach (var v in t.Vertices)
                {
                    str += v.KHID + " ";
                }
                str += "\n";
            }
            sr.Write(str);
        }
        public void WriteVertexs(StreamWriter sr)
        {
            string str = "";
            foreach (var t in Graph.Vertices)
            {
                str = t.KHID + " " + t.Attributes.Count.ToString() + " ";
                foreach (var key in t.Attributes.Keys)
                {
                    str += key + " " + ((t[key] == null) ? "*" : t[key].ToString()) + " ";
                }
                str += "\n";
            }
            sr.Write(str);
        }
        public void WriteEdges(StreamWriter sr)
        {
            string str = "";
            foreach (var t in Graph.Edges)
            {
                str = t.KHID + " " + t.Source.KHID + " " + t.Target.KHID + " ";
                foreach (var key in t.Attributes.Keys)
                {
                    str += key + " " + ((t[key] == null) ? "*" : t[key].ToString()) + " ";
                }
                str += "\n";
            }
            sr.Write(str);
        }
    }
}
