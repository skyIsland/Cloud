﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;
using NewLife.Log;
using NewLife.Web;
using NewLife.Data;
using XCode;
using XCode.Configuration;
using XCode.Membership;
using System.Web.Script.Serialization;

namespace Cloud.Domain
{
    /// <summary>管理员与角色映射表</summary>
    public partial class AdminRole : Entity<AdminRole>
    {
        #region 对象操作


        /// <summary>验证数据，通过抛出异常的方式提示验证失败。</summary>
        /// <param name="isNew"></param>
        public override void Valid(Boolean isNew)
        {
            // 如果没有脏数据，则不需要进行任何处理
            if (!HasDirty) return;

            // 这里验证参数范围，建议抛出参数异常，指定参数名，前端用户界面可以捕获参数异常并聚焦到对应的参数输入框
            //if (String.IsNullOrEmpty(Name)) throw new ArgumentNullException(_.Name, _.Name.DisplayName + "无效！");
            //if (!isNew && ID < 1) throw new ArgumentOutOfRangeException(_.ID, _.ID.DisplayName + "必须大于0！");

            // 建议先调用基类方法，基类方法会对唯一索引的数据进行验证
            base.Valid(isNew);

            // 在新插入数据或者修改了指定字段时进行唯一性验证，CheckExist内部抛出参数异常
            //if (isNew || Dirtys[__.Name]) CheckExist(__.Name);

        }

        /// <summary>首次连接数据库时初始化数据，仅用于实体类重载，用户不应该调用该方法</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override void InitData()
        {
            base.InitData();

            // InitData一般用于当数据表没有数据时添加一些默认数据，该实体类的任何第一次数据库操作都会触发该方法，默认异步调用
            // Meta.Count是快速取得表记录数
            if (Meta.Count > 0) return;

            // 需要注意的是，如果该方法调用了其它实体类的首次数据库操作，目标实体类的数据初始化将会在同一个线程完成
            if (XTrace.Debug) XTrace.WriteLine("开始初始化{0}[{1}]数据……", typeof(AdminRole).Name, Meta.Table.DataTable.DisplayName);

            //初始化数据
            var entity = new AdminRole();
            entity.AdminID = 1;
            entity.RoleID = 1;
            entity.Insert();
            if (XTrace.Debug) XTrace.WriteLine("完成初始化{0}[{1}]数据！", typeof(AdminRole).Name, Meta.Table.DataTable.DisplayName);
        }


        ///// <summary>已重载。基类先调用Valid(true)验证数据，然后在事务保护内调用OnInsert</summary>
        ///// <returns></returns>
        //public override Int32 Insert()
        //{
        //    return base.Insert();
        //}

        ///// <summary>已重载。在事务保护范围内处理业务，位于Valid之后</summary>
        ///// <returns></returns>
        //protected override Int32 OnInsert()
        //{
        //    return base.OnInsert();
        //}

        #endregion

        #region 扩展属性

        [NonSerialized]
        private Admin _Admin;
        /// <summary>该管理员与角色映射表所对应的管理员</summary>
        [XmlIgnore, ScriptIgnore]
        public Admin Admin
        {
            get
            {
                if (_Admin == null && AdminID > 0 && !Dirtys.ContainsKey("Admin"))
                {
                    _Admin = Admin.FindByID(AdminID);
                    Dirtys["Admin"] = true;
                }
                return _Admin;
            }
            set { _Admin = value; }
        }

        /// <summary>该管理员与角色映射表所对应的管理员名称</summary>
        [XmlIgnore, ScriptIgnore]
        public String AdminName { get { return Admin != null ? Admin.Name : String.Empty; } }

        [NonSerialized]
        private Role _Role;
        /// <summary>该管理员与角色映射表所对应的角色</summary>
        [XmlIgnore, ScriptIgnore]
        public Role Role
        {
            get
            {
                if (_Role == null && RoleID > 0 && !Dirtys.ContainsKey("Role"))
                {
                    _Role = Role.FindByID(RoleID);
                    Dirtys["Role"] = true;
                }
                return _Role;
            }
            set { _Role = value; }
        }

        /// <summary>该管理员与角色映射表所对应的角色名称</summary>
        [XmlIgnore, ScriptIgnore]
        public String RoleName { get { return Role != null ? Role.Name : String.Empty; } }

        #endregion

        #region 扩展查询

        /// <summary>根据主键ID查找</summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public static AdminRole FindByID(Int32 id)
        {
            if (Meta.Count >= 1000)
                return Find(__.ID, id);
            else // 实体缓存
                return Meta.Cache.Entities.Find(__.ID, id);
            // 单对象缓存
            //return Meta.SingleCache[id];
        }

        /// <summary>根据管理员查找</summary>
        /// <param name="adminid">管理员</param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public static EntityList<AdminRole> FindAllByAdminID(Int32 adminid)
        {
            if (Meta.Count >= 1000)
                return FindAll(__.AdminID, adminid);
            else // 实体缓存
                return Meta.Cache.Entities.FindAll(__.AdminID, adminid);
        }

        /// <summary>根据角色查找</summary>
        /// <param name="roleid">角色</param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public static EntityList<AdminRole> FindAllByRoleID(Int32 roleid)
        {
            if (Meta.Count >= 1000)
                return FindAll(__.RoleID, roleid);
            else // 实体缓存
                return Meta.Cache.Entities.FindAll(__.RoleID, roleid);
        }

        /// <summary>根据角色ID集合查找</summary>
        /// <param name="roleid">角色ID集合</param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public static EntityList<AdminRole> FindAllByRoleIDs(List<Int32> roleids)
        {
            var exp = _.RoleID.In(roleids);
            return FindAll(exp, null, null, 0, 0);
        }

        #endregion

        #region 高级查询
        // 以下为自定义高级查询的例子

        /// <summary>查询满足条件的记录集，分页、排序</summary>
        /// <param name="userid">用户编号</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="key">关键字</param>
        /// <param name="param">分页排序参数，同时返回满足条件的总记录数</param>
        /// <returns>实体集</returns>
        public static EntityList<AdminRole> Search(Int32 userid, DateTime start, DateTime end, String key, PageParameter param)
        {
            // WhereExpression重载&和|运算符，作为And和Or的替代
            // SearchWhereByKeys系列方法用于构建针对字符串字段的模糊搜索，第二个参数可指定要搜索的字段
            var exp = SearchWhereByKeys(key, null, null);

            // 以下仅为演示，Field（继承自FieldItem）重载了==、!=、>、<、>=、<=等运算符
            //if (userid > 0) exp &= _.OperatorID == userid;
            //if (isSign != null) exp &= _.IsSign == isSign.Value;
            //exp &= _.OccurTime.Between(start, end); // 大于等于start，小于end，当start/end大于MinValue时有效

            return FindAll(exp, param);
        }
        #endregion

        #region 扩展操作
        #endregion

        #region 业务
        #endregion
    }
}