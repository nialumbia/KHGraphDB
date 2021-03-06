﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KHGraphDB.Algorithm;
using KHGraphDB.Structure;
using KHGraphDB.Structure.Interface;

namespace KHGraphDB.Helper
{
    public class GraphHelper : KHGraphDB.Helper.IGraphHelper 
    {
        /// <summary>
        /// 获取和设置相关联的图
        /// </summary>
        IGraph Graph { get { return _Graph; } set { _Graph = value; } }
        IGraph _Graph;

        public GraphHelper(IGraph g)
        {
            Graph = g;
        }

        #region Vertex

        #region Add

        public IVertex AddVertex(IVertex v, KHGraphDB.Structure.Type type = null)
        {
            if (v != null && Graph.Equals(v.Graph)) return v;

            if (Graph.AddVertex(v, type))
                return v;

            return null;
        }

        public IVertex AddVertex(IDictionary<string, object> theAttributes, KHGraphDB.Structure.Type type = null)
        {
            return AddVertex(null, theAttributes, type);
        }

        public IVertex AddVertex(string ID, IDictionary<string, object> theAttributes, KHGraphDB.Structure.Type type = null)
        {
            IVertex v = new Vertex(ID, theAttributes);
            return AddVertex(v, type);
        }

        #endregion

        #region Remove
        public bool RemoveVertex(string ID)
        {
            IVertex v = Graph.Vertices.SingleOrDefault(m => m.KHID == ID);
            return RemoveVertex(v);
        }

        public bool RemoveVertex(IVertex v)
        {
            return Graph.RemoveVertex(v);
        }

        public IEnumerable<IVertex> RemoveVertices(IEnumerable<IVertex> vs)
        {
            return Graph.RemoveVertices(vs);
        }
        #endregion

        #region select

        public IEnumerable<IVertex> Verteics { get { return from v in Graph.Vertices select v; } }

        public IVertex SelectSingleVertex(string ID)
        {
            return Graph.Vertices.SingleOrDefault(m => m.KHID == ID);
        }

        public IVertex SelectSingleVertex(string key, object value)
        {
            return Graph.Vertices.SingleOrDefault(m => m[key] == value);
        }

        public IEnumerable<IVertex> SelectVerteics(string key, object value, string orderbyKey = null, IEnumerable<IVertex> vertics = null)
        {
            return from v in (null == vertics) ? Graph.Vertices : vertics
                   where (value == null) ? v.Attributes.Keys.Contains(key) : value.Equals(v[key])
                   orderby (orderbyKey == null) ? null : v[orderbyKey]
                   select v;
        }

        public IEnumerable<IVertex> SelectVerteics(string key, object value, IType type,string orderbyKey = null , Func<IVertex,bool> compare = null)
        {
            if (null == type)
                return new HashSet<Vertex>();
            return from v in type.Vertices
                   where ((value == null) ? v.Attributes.Keys.Contains(key) : value.Equals(v[key])) &&
                         ((compare == null) ? true : compare(v))
                   orderby (orderbyKey == null) ? null : v[orderbyKey]
                   select v;
        }

        #endregion

        #endregion

        #region Edge

        #region Add

        public IEdge AddEdge(IEdge e)
        {
            if (e != null && Graph.Equals(e.Graph)) return e;
            if(Graph.AddEdge(e))
                return e;
            return null;
        }

        public IEdge AddEdge(IVertex vSource, IVertex vTarget, IDictionary<string, object> theAttributes = null)
        {
            return AddEdge(null, vSource, vTarget, theAttributes);
        }

        public IEdge AddEdge(string ID, IVertex vSource, IVertex vTarget, IDictionary<string, object> theAttributes = null)
        {
            IEdge e = new Edge(ID, vSource, vTarget, theAttributes);
            return AddEdge(e);
        }

        #endregion

        #region Remove
        public bool RemoveEdge(string ID)
        {
            IEdge e = Graph.Edges.SingleOrDefault(m => m.KHID == ID);
            return RemoveEdge(e);
        }

        public bool RemoveEdge(IEdge e)
        {
            return Graph.RemoveEdge(e);
        }

        public IEnumerable<IEdge> RemoveEdges(IEnumerable<IEdge> es)
        {
            return Graph.RemoveEdges(es);
        }
        #endregion

        #region select

        public IEnumerable<IEdge> Edges { get { return from e in Graph.Edges select e; } }

        public IEdge SelectSingleEdge(string ID)
        {
            return Graph.Edges.SingleOrDefault(m => m.KHID == ID);
        }

        public IEdge SelectSingleEdge(string key, object value)
        {
            return Graph.Edges.SingleOrDefault(m => m[key] == value);
        }

        public IEnumerable<IEdge> SelectEdges(string key, object value, string orderbyKey = null, IEnumerable<IEdge> edges = null, Func<IEdge,object,bool> compare = null)
        {
            return from e in (edges == null) ? Graph.Edges : edges
                   where ((value == null) ? e.Attributes.Keys.Contains(key) : value.Equals(e[key])) &&
                         ((compare == null) ? true : compare(e,value))
                   orderby (orderbyKey == null) ? null : e[orderbyKey]
                   select e;
        }

        public IEnumerable<IEdge> SelectEdges(string key, object value, IVertex vSource, IVertex vTarget, string orderbyKey = null, IEnumerable<IEdge> edges = null, Func<IEdge,bool> compare = null)
        {
            return from e in (edges == null) ? Graph.Edges : edges
                   where ((value == null) ? e.Attributes.Keys.Contains(key) : value.Equals(e[key])) &&
                         ((vSource == null) ? true : e.Source.Equals(vSource)) &&
                         ((vTarget == null) ? true : e.Target.Equals(vTarget)) &&
                         ((compare == null) ? true : compare(e))
                   orderby (orderbyKey == null) ? null : e[orderbyKey]
                   select e;
        }//三元运算符优先级问题

        public IEnumerable<IEdge> SelectParallelEdges(IEdge edge, string orderbyKey = null, IEnumerable<IEdge> edges = null)
        {
            return SelectParallelEdges(edge.Source, edge.Target, orderbyKey, edges);
        }

        public IEnumerable<IEdge> SelectParallelEdges(IVertex vSource, IVertex vTarget, string orderbyKey = null,IEnumerable<IEdge> edges = null)
        {
            if (vSource == null || vTarget == null ) return new HashSet<Edge>();
            if ( vSource.OutDegree < vTarget.InDegree)
                return from e in vSource.OutgoingEdges
                       where e.Target.Equals(vTarget) && 
                            ((edges == null) ? true : edges.Contains(e))
                       orderby (orderbyKey == null) ? null : e[orderbyKey]
                       select e;
            else
                return from e in vTarget.IncomingEdges
                       where e.Source.Equals(vSource) && 
                            ((edges == null) ? true : edges.Contains(e))
                       orderby (orderbyKey == null) ? null : e[orderbyKey]
                       select e;
        }

        #endregion

        #endregion

        #region Type

        #region Add

        public IType AddType(IType t)
        {
            if (Graph.AddType(t))
                return t;
            return null;
        }

        public IType AddType(string Name)
        {
            return AddType(null, Name);
        }

        public IType AddType(string ID, string Name, IDictionary<string, object> theAttributes = null)
        {
            if (SelectSingleTypeName(Name) != null) return null;
            if (theAttributes == null)
            {
                IType t = new KHGraphDB.Structure.Type(ID);
                t.Name = Name;
                return AddType(t);
            }
            else
            {
                IType t = new KHGraphDB.Structure.Type(ID, theAttributes);
                t.Name = Name;
                return AddType(t);
            }
        }

        #endregion

        #region Remove
        public bool RemoveType(string ID)
        {
            IType t = Graph.Types.SingleOrDefault(m => m.KHID == ID);
            return RemoveType(t);
        }

        public bool RemoveTypeByName(string Name)
        {
            IType t = Graph.Types.SingleOrDefault(m => m.Name == Name);
            return RemoveType(t);
        }

        public bool RemoveType(IType t)
        {
            return Graph.RemoveType(t);
        }

        public IEnumerable<IType> RemoveTypes(IEnumerable<IType> ts)
        {
            return Graph.RemoveTypes(ts);
        }

        #endregion

        #region select

        public IEnumerable<IType> Types { get { return from t in Graph.Types select t; } }

        public IType SelectSingleType(string ID)
        {
            return Graph.Types.SingleOrDefault(m => m.KHID == ID);
        }

        public IType SelectSingleType(string key, object value)
        {
            return Graph.Types.SingleOrDefault(m => m[key] == value);
        }

        public IType SelectSingleTypeName(string Name)
        {
            return Graph.Types.SingleOrDefault(m => m.Name == Name);
        }

        public IEnumerable<IType> SelectTypes(string key, object value, string orderbyKey = null, IEnumerable<IType> types = null, Func<IType,bool> compare = null)
        {
            return from t in (types == null) ? Graph.Types : types
                       where ((value == null) ? t.Attributes.Keys.Contains(key) : value.Equals(t[key])) &&
                             ((compare == null) ? true : compare(t) )
                       orderby (orderbyKey == null) ? null : t[orderbyKey]
                       select t;
        }


        #endregion

        #endregion

        #region Algorithm

        BreadthFirstSearch bfs = new BreadthFirstSearch();

        public IEnumerable<IVertex> FindPathVertex(IVertex vSource, IVertex vTarget)
        {
            return bfs.Search(Graph, vSource, vTarget);
        }

        public IEnumerable<IVertex> FindPathVertexAttrExist(IVertex vSource, IVertex vTarget, string AttrKey)
        {
            Func<IVertex, bool> exist = v => v.Attributes.Keys.Contains(AttrKey);
            return FindPathVertexAdapt(vSource, vTarget, exist);
        }

        public IEnumerable<IVertex> FindPathVertexAttrExist(IVertex vSource, IVertex vTarget, string AttrKey, object value)
        {
            Func<IVertex, bool> exist = v => v[AttrKey] == value;
            return FindPathVertexAdapt(vSource, vTarget, exist);
        }

        public IEnumerable<IVertex> FindPathVertexAdapt(IVertex vSource, IVertex vTarget, Func<IVertex, bool> Adapter)
        {
            return bfs.Search(Graph, vSource, vTarget, Adapter);
        }

        public IEnumerable<IDBObject> FindPath(IVertex vSource, IVertex vTarget)
        {
            return bfs.Search(Graph, vSource, vTarget);
        }

        #endregion

    }
}
