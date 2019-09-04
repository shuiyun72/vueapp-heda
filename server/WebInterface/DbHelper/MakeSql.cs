using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Tool.DB
{
    /// <summary>
    /// 类  名：生成Sql语句
    /// 
    /// </summary>
    public partial class MakeSql
    {
        /// <summary>
        /// 静态qlDbHelper 对象，注意需要赋值
        /// </summary>
        //public Tool.DB.SqlDbHelper DbHelper;
        //public static Tool.DB.SqlDbHelper DbHelper;
        /// <summary>
        /// 构造函数初始化
        /// </summary>
        public MakeSql()
        {
            //if (DbHelper == null)
            //{
            //    DbHelper = new SqlDbHelper(ConfigurationManager.AppSettings["ConnStr"].ToString());
            //}
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sStringType">SQL语句类型</param>
        /// <param name="dbName">操作表名称</param>
        public MakeSql(SqlStringType sStringType, string dbName)
        {
            //if (DbHelper == null)
            //{
            //    DbHelper = new SqlDbHelper(ConfigurationManager.AppSettings["ConnStr"].ToString());
            //}
            _SqlStringType = sStringType;
            _DBName = dbName;
        }
        private string _SelectFieldString = "";

        /// <summary>
        /// 选择字段字符串，SQL语句格式,注意赋值后，AddField方法添加的字段将失效；
        /// </summary>
        public string SelectFieldString
        {
            get { return _SelectFieldString; }
            set { _SelectFieldString = value; }
        }
        private string _ConditionString = "";

        /// <summary>
        /// 条件字段字符串，SQL语句格式,注意赋值后，AddCondition方法添加的字段将失效；
        /// </summary>
        public string ConditionString
        {
            get { return _ConditionString; }
            set { _ConditionString = value; }
        }
        private string _OrderByString = "";

        /// <summary>
        /// 排序字段字符串，SQL语句格式,注意赋值后，AddOrderBy方法添加的字段将失效
        /// </summary>
        public string OrderByString
        {
            get { return _OrderByString; }
            set { _OrderByString = value; }
        }

        private int _Count;
        /// <summary>
        ///  查询的结果总数
        /// </summary>
        public int Count
        {
            get { return _Count; }
            set { _Count = value; }
        }

        private int _TopNum = 0;
        /// <summary>
        /// 查询记录前几行
        /// </summary>
        public int TopNum
        {
            get
            {
                return _TopNum;
            }
            set
            {
                _TopNum = value;
            }
        }


        private List<sOrderBy> _OrderBy = new List<sOrderBy>();

        /// <summary>
        /// 排序字段List
        /// </summary>
        public List<sOrderBy> OrderBy
        {
            get { return _OrderBy; }
            set { _OrderBy = value; }
        }
        private SqlStringType _SqlStringType;
        /// <summary>
        /// SQL语句类型
        /// </summary>
        public SqlStringType SqlStringType
        {
            get { return _SqlStringType; }
            set { _SqlStringType = value; }
        }
        private string _DBName;

        /// <summary>
        /// 操作表名称
        /// </summary>
        public string DBName
        {
            get { return _DBName; }
            set { _DBName = value; }
        }
        private List<sField> _SqlField = new List<sField>();

        /// <summary>
        /// 字段名称List
        /// </summary>
        public List<sField> SqlField
        {
            get { return _SqlField; }
            set { _SqlField = value; }
        }
        private List<sCondition> _SqlCondition = new List<sCondition>();

        /// <summary>
        ///  查询条件集合List
        /// </summary>
        public List<sCondition> SqlCondition
        {
            get { return _SqlCondition; }
            set { _SqlCondition = value; }
        }

        /// <summary>
        /// 输出SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            base.ToString();
            return OutSqlString();
        }

        /// <summary>
        /// 输出SQL语句
        /// </summary>
        /// <returns></returns>
        public string OutSqlString()
        {
            string SqlString = "";
            switch (_SqlStringType)
            {
                case SqlStringType.SELECT:
                    SqlString = OutSelectSQL();
                    break;
                case SqlStringType.INSERT:
                    SqlString = OutInsertSQL();
                    break;
                case SqlStringType.UPDATE:
                    SqlString = OutUpdateSQL();
                    break;
                case SqlStringType.DELETE:
                    SqlString = OutDelSQL();
                    break;
            }
            return SqlString;
        }
        private string GetTopString()
        {
            string SqlString = "";
            if (_TopNum != 0)
            {
                SqlString += " TOP ";
                SqlString += _TopNum;
            }
            return SqlString;
        }
        /// <summary>
        /// 获得查询字符串SQL
        /// </summary>
        /// <returns></returns>
        public string GetOrderByString()
        {
            StringBuilder SqlString = new StringBuilder();
            SqlString.Append(" ORDER BY ");
            if (!string.IsNullOrEmpty(_OrderByString.Trim())) { return " ORDER BY " + _OrderByString; }
            int cNum = _OrderBy.Count; ;
            if (cNum == 0) { return ""; }

            for (int i = 0; i < cNum; i++)
            {
                SqlString.Append(_OrderBy[i].FieldName);
                SqlString.Append("  ");
                if (_OrderBy[i].OrderByType == OrderByType.DESC)
                {
                    SqlString.Append(" DESC ");
                }
                else
                {
                    SqlString.Append(" ASC ");
                }
                SqlString.Append(" ");

                if (i == (cNum - 1))
                {
                    SqlString.Append(" ");
                }
                else
                {
                    SqlString.Append(" , ");
                }
            }
            return SqlString.ToString();
        }


        /// <summary>
        /// 获得条件字符串SQL
        /// </summary>
        /// <returns></returns>
        public string GetConditionString()
        {
            StringBuilder SqlString = new StringBuilder();
            SqlString.Append(" WHERE ");
            if (!string.IsNullOrEmpty(_ConditionString.Trim())) { return " WHERE " + _ConditionString; }
            int cNum = _SqlCondition.Count;
            if (cNum == 0) { return ""; }

            for (int i = 0; i < cNum; i++)
            {
                SqlString.Append(_SqlCondition[i].FieldName);
                SqlString.Append(" ");
                SqlString.Append(_SqlCondition[i].Sign);

                if (_SqlCondition[i].Sign.Trim().ToLower() == "in")
                {
                    SqlString.Append(" (");
                    SqlString.Append(_SqlCondition[i].FieldValue);
                    SqlString.Append(") ");
                }
                else if (_SqlCondition[i].Sign.Trim().ToLower() == "like")
                {
                    SqlString.Append(" '%");
                    SqlString.Append(_SqlCondition[i].FieldValue);
                    SqlString.Append("%' ");
                }
                else
                {
                    if (_SqlCondition[i].FieldType == FieldType.INT)
                    {
                        SqlString.Append(" ");
                        SqlString.Append(_SqlCondition[i].FieldValue);
                    }
                    else
                    {
                        SqlString.Append(" '");
                        SqlString.Append(_SqlCondition[i].FieldValue);
                        SqlString.Append("' ");
                    }
                }
                if (i == (cNum - 1))
                {
                    SqlString.Append(" ");
                }
                else
                {
                    SqlString.Append(" ");
                    SqlString.Append(GetJoinSign(_SqlCondition[i].JoinSign));
                    SqlString.Append(" ");
                }
            }
            return SqlString.ToString();
        }
        /// <summary>
        /// 获得查询字符串SQL
        /// </summary>
        /// <returns></returns>
        public string GetConditionURLString()
        {
            StringBuilder SqlString = new StringBuilder();
            if (!string.IsNullOrEmpty(_ConditionString.Trim())) { return ""; }
            int cNum = _SqlCondition.Count;
            if (cNum == 0) { return ""; }

            for (int i = 0; i < cNum; i++)
            {
                SqlString.Append("&");
                SqlString.Append(_SqlCondition[i].FieldName);
                SqlString.Append("=");
                SqlString.Append(_SqlCondition[i].FieldValue);
            }
            return SqlString.ToString();
        }

        /// <summary>
        /// 获得查询条件字符串SQL 以Command方式
        /// </summary>
        /// <returns></returns>
        public string GetConditionCommandString()
        {
            StringBuilder SqlString = new StringBuilder();
            SqlString.Append(" WHERE ");
            if (!string.IsNullOrEmpty(_ConditionString.Trim())) { return " WHERE " + _ConditionString; }
            int cNum = _SqlCondition.Count;
            if (cNum == 0) { return ""; }

            for (int i = 0; i < cNum; i++)
            {
                SqlString.Append(_SqlCondition[i].FieldName);
                SqlString.Append(" ");
                SqlString.Append(_SqlCondition[i].Sign);

                if (_SqlCondition[i].Sign.Trim().ToLower() == "in")
                {
                    SqlString.Append(" (");
                    SqlString.Append(_SqlCondition[i].FieldValue);
                    SqlString.Append(") ");
                }
                else if (_SqlCondition[i].Sign.Trim().ToLower() == "like")
                {
                    SqlString.Append(" '%");
                    SqlString.Append(_SqlCondition[i].FieldValue);
                    SqlString.Append("%' ");
                }
                else
                {
                    SqlString.Append(" @");
                    SqlString.Append(_SqlCondition[i].FieldName);
                }
                if (i == (cNum - 1))
                {
                    SqlString.Append(" ");
                }
                else
                {
                    SqlString.Append(" ");
                    SqlString.Append(GetJoinSign(_SqlCondition[i].JoinSign));
                    SqlString.Append(" ");
                }
            }
            return SqlString.ToString();
        }
        private string GetJoinSign(JoinSign sJoinSign)
        {
            if (sJoinSign == JoinSign.OR)
            {
                return " OR ";
            }
            else
            {
                return " AND ";
            }
        }
        /// <summary>
        /// 输出Select查询SQL语句
        /// </summary>
        /// <returns></returns>
        public string OutSelectSQL()
        {
            if (string.IsNullOrEmpty(_DBName.Trim())) { return ""; }

            StringBuilder SqlString = new StringBuilder();
            SqlString.Append(" SELECT ");
            if (string.IsNullOrEmpty(_SelectFieldString.Trim()))
            {
                int fNum = _SqlField.Count;
                if (fNum == 0)
                {
                    SqlString.Append(GetTopString());
                    SqlString.Append(" * ");
                }
                else
                {
                    SqlString.Append(GetTopString());
                    for (int i = 0; i < fNum; i++)
                    {
                        SqlString.Append(_SqlField[i].FieldName);
                        if (i == (fNum - 1))
                        {
                            SqlString.Append(" ");
                        }
                        else
                        {
                            SqlString.Append(",");
                        }
                    }
                }
            }
            else
            {
                SqlString.Append(_SelectFieldString);
            }
            SqlString.Append("  FROM ");
            SqlString.Append(_DBName);
            SqlString.Append(GetConditionString());
            SqlString.Append(GetOrderByString());
            return SqlString.ToString();
        }
        
        /// <summary>
        /// 输出Update更新SQL语句
        /// </summary>
        /// <returns></returns>
        public string OutUpdateSQL()
        {
            if (string.IsNullOrEmpty(_DBName.Trim())) { return ""; }
            StringBuilder SqlString = new StringBuilder();
            int fNum = _SqlField.Count;
            if (fNum == 0)
            {
                SqlString.Append(" ");
            }
            else
            {
                SqlString.Append(" UPDATE ");
                SqlString.Append(_DBName);
                SqlString.Append(" SET ");
                for (int i = 0; i < fNum; i++)
                {
                    SqlString.Append(_SqlField[i].FieldName);
                    SqlString.Append(" = ");
                    if (_SqlField[i].FieldType == FieldType.INT)
                    {
                        SqlString.Append(_SqlField[i].FieldValue);
                    }
                    else
                    {
                        SqlString.Append(" '");
                        SqlString.Append(_SqlField[i].FieldValue);
                        SqlString.Append("' ");
                    }
                    if (i == (fNum - 1))
                    {
                        SqlString.Append(" ");
                    }
                    else
                    {
                        SqlString.Append(",");
                    }
                }
                SqlString.Append(GetConditionString());
            }
            return SqlString.ToString();
        }
       
        /// <summary>
        /// 输出Del删除SQL语句
        /// </summary>
        /// <returns></returns>
        public string OutDelSQL()
        {
            if (string.IsNullOrEmpty(_DBName.Trim())) { return ""; }
            StringBuilder SqlString = new StringBuilder();
            int fNum = _SqlCondition.Count;
            if (fNum == 0)
            {
                SqlString.Append(" ");
            }
            else
            {
                SqlString.Append(" DELETE FROM ");
                SqlString.Append(_DBName);
                SqlString.Append(" ");
                SqlString.Append(GetConditionString());
            }

            return SqlString.ToString();
        }
       

        /// <summary>
        /// 输出insert插入SQL语句
        /// </summary>
        /// <returns></returns>
        public string OutInsertSQL()
        {
            if (string.IsNullOrEmpty(_DBName.Trim())) { return ""; }
            StringBuilder SqlString = new StringBuilder();
            StringBuilder SqlString2 = new StringBuilder();
            int fNum = _SqlField.Count;
            if (fNum == 0)
            {
                SqlString.Append(" ");
            }
            else
            {
                SqlString.Append(" INSERT INTO ");
                SqlString.Append(_DBName);
                SqlString.Append(" (");
                for (int i = 0; i < fNum; i++)
                {
                    SqlString.Append(_SqlField[i].FieldName);
                    if (_SqlField[i].FieldType == FieldType.INT)
                    {
                        SqlString2.Append(_SqlField[i].FieldValue);
                    }
                    else
                    {
                        SqlString2.Append(" '");
                        SqlString2.Append(_SqlField[i].FieldValue);
                        SqlString2.Append("' ");
                    }
                    if (i == (fNum - 1))
                    {
                        SqlString.Append(" ");
                        SqlString2.Append(" ");
                    }
                    else
                    {
                        SqlString.Append(",");
                        SqlString2.Append(", ");
                    }
                }
                SqlString.Append(") VALUES (");
                SqlString.Append(SqlString2.ToString());
                SqlString.Append(")");
            }
            return SqlString.ToString();
        }

        /// <summary>
        /// 添加SQL语句条件
        /// </summary>
        /// <param name="FieldName">排序字段</param>
        /// <param name="OrderByType">排序类型</param>
        public void AddOrderBy(string FieldName, OrderByType OrderByType)
        {
            if (string.IsNullOrEmpty(FieldName)) { return; }
            sOrderBy newOrderBy = new sOrderBy(FieldName, OrderByType);
            _OrderBy.Add(newOrderBy);
        }

        /// <summary>
        /// 添加SQL语句查询字段，注意为空时，字段为* 全部
        /// </summary>
        /// <param name="FieldName">字段名</param>
        /// <param name="FieldValue">字段值</param>
        /// <param name="FieldType">字段类型</param>
        public void AddField(string FieldName, string FieldValue, FieldType FieldType)
        {
            if (string.IsNullOrEmpty(FieldName)) { return; }
            if (string.IsNullOrEmpty(FieldValue) && _SqlStringType != SqlStringType.SELECT) { return; }
            sField newField = new sField(FieldName, FieldValue, FieldType);
            _SqlField.Add(newField);
        }
        /// <summary>
        /// 添加SQL语句查询字段，注意为空时，字段为* 全部
        /// </summary>
        /// <param name="FieldName">字段名</param>
        /// <param name="FieldValue">字段值</param>
        /// <param name="FieldType">字段类型</param>
        public void AddField(string FieldName, object FieldValue, FieldType FieldType)
        {
            if (FieldValue == null) { return; }
            AddField(FieldName, FieldValue.ToString(), FieldType.STR);
        }
        /// <summary>
        /// 添加SQL语句查询字段，注意为空时，字段为* 全部
        /// </summary>
        /// <param name="FieldName">字段名</param>
        /// <param name="FieldValue">字段值</param>
        public void AddField(string FieldName, object FieldValue)
        {
            if (FieldValue == null) { return; }
            AddField(FieldName, FieldValue.ToString(), FieldType.STR);
        }
        /// <summary>
        /// 添加SQL语句查询字段，注意为空时，字段为* 全部
        /// </summary>
        /// <param name="FieldName">字段名</param>
        /// <param name="FieldValue">字段值</param>
        public void AddField(string FieldName, int FieldValue)
        {
            AddField(FieldName, FieldValue.ToString(), FieldType.INT);
        }
        /// <summary>
        /// 添加SQL语句查询字段，注意为空时，字段为* 全部
        /// </summary>
        /// <param name="FieldName">字段名</param>
        /// <param name="FieldValue">字段值</param>
        public void AddField(string FieldName, string FieldValue)
        {
            AddField(FieldName, FieldValue, FieldType.STR);
        }
        /// <summary>
        /// 添加SQL语句查询字段，注意字段值为“”
        /// </summary>
        /// <param name="FieldName">字段名</param>
        public void AddField(string FieldName)
        {
            AddField(FieldName, "", FieldType.STR);
        }


        /// <summary>
        /// 添加SQL语句条件字段
        /// </summary>
        /// <param name="FieldName">字段名称</param>
        /// <param name="FieldValue">条件值</param>
        /// <param name="Sign">文本格式条件，如“>,=,<”</param>
        /// <param name="FieldType">字段类型</param>
        /// <param name="JoinSign">条件</param>
        public void AddCondition(string FieldName, string FieldValue, string Sign, FieldType FieldType, JoinSign JoinSign)
        {
            if (string.IsNullOrEmpty(FieldName)) { return; }
            if (string.IsNullOrEmpty(FieldValue)) { return; }
            if (string.IsNullOrEmpty(Sign)) { Sign = " = "; }
            sCondition newCondition = new sCondition(FieldName, FieldValue, Sign, FieldType, JoinSign);
            _SqlCondition.Add(newCondition);
        }

        /// <summary>
        /// 添加SQL语句条件字段
        /// </summary>
        /// <param name="FieldName">字段名称</param>
        /// <param name="FieldValue">条件值</param>
        /// <param name="FieldType">字段类型</param>
        /// <param name="JoinSign">条件</param>
        public void AddCondition(string FieldName, object FieldValue, FieldType FieldType, JoinSign JoinSign)
        {
            if (FieldValue == null) { return; }
            AddCondition(FieldName, FieldValue.ToString(), " = ", FieldType, JoinSign.AND);
        }
        /// <summary>
        /// 添加SQL语句条件字段
        /// </summary>
        /// <param name="FieldName">字段名称</param>
        /// <param name="FieldValue">字段类型</param>
        /// <param name="FieldType">条件</param>
        public void AddCondition(string FieldName, object FieldValue, FieldType FieldType)
        {
            if (FieldValue == null) { return; }
            AddCondition(FieldName, FieldValue.ToString(), " = ", FieldType, JoinSign.AND);
        }
        /// <summary>
        /// 添加SQL语句条件字段
        /// </summary>
        /// <param name="FieldName">字段名称</param>
        /// <param name="FieldValue">字段类型</param>
        /// <param name="FieldType">条件</param>
        public void AddCondition(string FieldName, string FieldValue, FieldType FieldType)
        {
            AddCondition(FieldName, FieldValue, " = ", FieldType, JoinSign.AND);
        }
        /// <summary>
        /// 添加SQL语句条件字段
        /// </summary>
        /// <param name="FieldName">字段名称</param>
        /// <param name="FieldValue">字段类型</param>
        /// <param name="FieldType">条件</param>
        public void AddCondition(string FieldName, int FieldValue, FieldType FieldType)
        {
            AddCondition(FieldName, FieldValue.ToString(), " = ", FieldType.INT, JoinSign.AND);
        }
        /// <summary>
        /// 添加SQL语句条件字段
        /// </summary>
        /// <param name="FieldName">字段名称</param>
        /// <param name="FieldType">条件</param>
        public void AddCondition(string FieldName, string FieldValue)
        {
            AddCondition(FieldName, FieldValue, " = ", FieldType.STR, JoinSign.AND);
        }
        /// <summary>
        /// 添加SQL语句条件字段
        /// </summary>
        /// <param name="FieldName">字段名称</param>
        /// <param name="FieldValue">字段类型</param>
        public void AddCondition(string FieldName, int FieldValue)
        {
            AddCondition(FieldName, FieldValue.ToString(), " = ", FieldType.INT, JoinSign.AND);
        }
        /// <summary>
        /// 添加SQL语句条件字段
        /// </summary>
        /// <param name="FieldName">字段名称</param>
        /// <param name="FieldValue">字段类型</param>
        public void AddCondition(string FieldName, object FieldValue)
        {
            if (FieldValue == null) { return; }
            AddCondition(FieldName, FieldValue.ToString(), " = ", FieldType.STR, JoinSign.AND);
        }

        /// <summary>
        /// 添加SQL语句条件字段 Like 条件
        /// </summary>
        /// <param name="FieldName">字段名称</param>
        /// <param name="FieldValue">字段类型</param>
        /// <param name="JoinSign">条件</param>
        public void AddLikeCondition(string FieldName, string FieldValue, JoinSign JoinSign)
        {
            if (string.IsNullOrEmpty(FieldName)) { return; }
            if (string.IsNullOrEmpty(FieldValue)) { return; }
            sCondition newCondition = new sCondition(FieldName, FieldValue, " like ", FieldType.STR, JoinSign);
            _SqlCondition.Add(newCondition);
        }
        /// <summary>
        /// 添加SQL语句条件字段 Like 条件
        /// </summary>
        /// <param name="FieldName">字段名称</param>
        /// <param name="FieldValue">字段类型</param>
        public void AddLikeCondition(string FieldName, string FieldValue)
        {
            if (string.IsNullOrEmpty(FieldName)) { return; }
            if (string.IsNullOrEmpty(FieldValue)) { return; }
            sCondition newCondition = new sCondition(FieldName, FieldValue, " like ", FieldType.STR, JoinSign.AND);
            _SqlCondition.Add(newCondition);
        }
        /// <summary>
        /// 通过DR 设置SQL语句字段
        /// </summary>
        /// <param name="dcc">DataColumnCollection 构架</param>
        /// <param name="dr">DataRow</param>
        public void SetField(DataColumnCollection dcc, DataRow dr)
        {
            SetField(dcc, dr, "");
        }
        /// <summary>
        /// 通过DR 设置SQL语句字段
        /// </summary>
        /// <param name="dcc">DataColumnCollection 构架</param>
        /// <param name="dr">DataRow</param>
        /// <param name="NoShowFiled">不显示字段</param>
        public void SetField(DataColumnCollection dcc, DataRow dr, string NoShowFiled)
        {
            NoShowFiled = "," + NoShowFiled + ",";
            string FiledName = "";
            for (int i = 0; i < dcc.Count; i++)
            {
                FiledName = dcc[i].ColumnName;
                if (NoShowFiled.IndexOf("," + FiledName + ",") < 0)
                {
                    AddField(FiledName, dr[i].ToString().Trim(), SetFieldType(dcc[i].DataType));
                }
            }
        }
        private FieldType SetFieldType(Type T)
        {
            FieldType FT;
            switch (T.ToString())
            {
                case "System.Decimal":
                    FT = FieldType.INT;
                    break;
                case "System.Int32":
                    FT = FieldType.INT;
                    break;
                case "System.String":
                    FT = FieldType.STR;
                    break;
                case "System.DateTime":
                    FT = FieldType.DATE;
                    break;
                default:
                    FT = FieldType.STR;
                    break;
            }
            return FT;
        }

       
               

        /// <summary>
        /// 执行QL语句,主要用于“分页查询”
        /// </summary>
        /// <param name="SQL">SQL语句</param>
        /// <param name="Ordering">排序字符串</param>
        /// <param name="UserNum">数量</param>
        /// <param name="UserListIndex">当前第几页</param>
        /// <returns></returns>
        public string GetSqlForSqlToPager(string SQL, string Ordering, int UserNum, int UserListIndex)
        {
            int beginNum = (UserListIndex - 1) * (UserNum);
            //if (beginNum == 0) { beginNum = 1; }
            int endNum = beginNum + UserNum;

            #region  添加于2015-05-15
            /*功能描述：添加指定显示列，默认显示全部
             */
            StringBuilder FieldColumn = new StringBuilder();
            FieldColumn.Append(" * ");
            if (string.IsNullOrEmpty(_SelectFieldString.Trim()))
            {
                int fNum = _SqlField.Count;
                if (fNum > 0)
                {
                    FieldColumn.Clear();
                }
                for (int i = 0; i < fNum; i++)
                {
                    FieldColumn.Append(_SqlField[i].FieldName);
                    if (i == (fNum - 1))
                    {
                        FieldColumn.Append(" ");
                    }
                    else
                    {
                        FieldColumn.Append(",");
                    }
                }
            }
            #endregion
            string sqlString = "SELECT * FROM(SELECT " + FieldColumn.ToString() + " , ROW_NUMBER() OVER ( Order by " + Ordering + ") AS Pos FROM (" + SQL + ") as T) AS TT where TT.Pos > " + beginNum.ToString() + " and TT.Pos <= " + endNum.ToString();
            //string sqlString = "SELECT * FROM(SELECT * , ROW_NUMBER() OVER ( Order by " + Ordering + ") AS Pos FROM (" + SQL + ") as T) AS TT where TT.Pos > " + beginNum.ToString() + " and TT.Pos <= " + endNum.ToString();

            return sqlString;
        }

        /// <summary>
        /// 执行配置的SQL语句,主要用于“分页查询”
        /// </summary>
        /// <param name="UserNum">数量</param>
        /// <param name="UserListIndex">当前第几页</param>
        /// <returns>DT数据集</returns>
        public string GetDataTableForSqlToPager(int UserNum, int UserListIndex)
        {
            int beginNum = (UserListIndex - 1) * (UserNum);
            //if (beginNum == 0) { beginNum = 1; }
            int endNum = beginNum + UserNum;
            string sqlString = "SELECT * FROM(SELECT * , ROW_NUMBER() OVER (" + GetOrderByString() + " ) AS Pos FROM " + DBName + " " + GetConditionString() + ") AS T where T.Pos > " + beginNum.ToString() + " and T.Pos <= " + endNum.ToString();

            //throw new Exception(sqlString);

            return sqlString;
        }

        


        /// <summary>
        /// 设置排序方式
        /// </summary>
        /// <param name="str">排序方式字符串，如“Desc，ASC”</param>
        /// <returns></returns>
        public static OrderByType SetOrderByType(string str)
        {
            str = str.ToLower();
            if (str == "Desc")
            {
                return OrderByType.DESC;
            }
            else
            {
                return OrderByType.ASC;
            }
        }
    }
    public enum JoinSign { AND, OR }
    public class sOrderBy
    {
        public sOrderBy(string fName, OrderByType OBT)
        {
            _FieldName = fName;
            _OrderByType = OBT;
        }

        private string _FieldName;

        public string FieldName
        {
            get { return _FieldName; }
            set { _FieldName = value; }
        }
        private OrderByType _OrderByType;

        public OrderByType OrderByType
        {
            get { return _OrderByType; }
            set { _OrderByType = value; }
        }
    }
    public enum OrderByType { DESC, ASC }
    public class sCondition
    {
        public sCondition(string fName, string fValue, string sSign, FieldType fType, JoinSign sJoinSign)
        {
            _FieldName = fName;
            _FieldValue = fValue;
            _FieldType = fType;
            _Sign = sSign;
            _JoinSign = sJoinSign;
        }
        private JoinSign _JoinSign;

        public JoinSign JoinSign
        {
            get { return _JoinSign; }
            set { _JoinSign = value; }
        }

        private string _Sign = " = ";

        public string Sign
        {
            get { return _Sign; }
            set { _Sign = value; }
        }
        private string _FieldName;

        public string FieldName
        {
            get { return _FieldName; }
            set { _FieldName = value; }
        }
        private string _FieldValue;

        public string FieldValue
        {
            get { return _FieldValue; }
            set { _FieldValue = value; }
        }
        private FieldType _FieldType;

        public FieldType FieldType
        {
            get { return _FieldType; }
            set { _FieldType = value; }
        }
    }
    public class sField
    {
        public sField(string fName, string fValue, FieldType fType)
        {
            _FieldName = fName;
            _FieldValue = fValue;
            _FieldType = fType;
        }
        private string _FieldName;

        public string FieldName
        {
            get { return _FieldName; }
            set { _FieldName = value; }
        }
        private string _FieldValue;

        public string FieldValue
        {
            get { return _FieldValue; }
            set { _FieldValue = value; }
        }
        private FieldType _FieldType;

        public FieldType FieldType
        {
            get { return _FieldType; }
            set { _FieldType = value; }
        }
    }
    public enum FieldType { STR, INT, DATE, FLOAT, SEQID }
    public enum SqlStringType { SELECT, INSERT, UPDATE, DELETE }
}