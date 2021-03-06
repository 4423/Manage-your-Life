﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.18444
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Manage_your_Life
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="ApplicationDatabase")]
	public partial class ApplicationDataClassesDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region 拡張メソッドの定義
    partial void OnCreated();
    partial void InsertDatabaseProcess(DatabaseProcess instance);
    partial void UpdateDatabaseProcess(DatabaseProcess instance);
    partial void DeleteDatabaseProcess(DatabaseProcess instance);
    partial void InsertDatabaseApplication(DatabaseApplication instance);
    partial void UpdateDatabaseApplication(DatabaseApplication instance);
    partial void DeleteDatabaseApplication(DatabaseApplication instance);
    partial void InsertDatabaseTimeline(DatabaseTimeline instance);
    partial void UpdateDatabaseTimeline(DatabaseTimeline instance);
    partial void DeleteDatabaseTimeline(DatabaseTimeline instance);
    partial void InsertDatabaseDate(DatabaseDate instance);
    partial void UpdateDatabaseDate(DatabaseDate instance);
    partial void DeleteDatabaseDate(DatabaseDate instance);
    #endregion
		
		public ApplicationDataClassesDataContext() : 
				base(global::Manage_your_Life.Properties.Settings.Default.ApplicationDatabaseConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public ApplicationDataClassesDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public ApplicationDataClassesDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public ApplicationDataClassesDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public ApplicationDataClassesDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<DatabaseProcess> DatabaseProcess
		{
			get
			{
				return this.GetTable<DatabaseProcess>();
			}
		}
		
		public System.Data.Linq.Table<DatabaseApplication> DatabaseApplication
		{
			get
			{
				return this.GetTable<DatabaseApplication>();
			}
		}
		
		public System.Data.Linq.Table<DatabaseTimeline> DatabaseTimeline
		{
			get
			{
				return this.GetTable<DatabaseTimeline>();
			}
		}
		
		public System.Data.Linq.Table<DatabaseDate> DatabaseDate
		{
			get
			{
				return this.GetTable<DatabaseDate>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.DatabaseProcess")]
	public partial class DatabaseProcess : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private string _Name;
		
		private string _Path;
		
		private int _AppId;
		
		private EntitySet<DatabaseApplication> _DatabaseApplication;
		
    #region 拡張メソッドの定義
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnPathChanging(string value);
    partial void OnPathChanged();
    partial void OnAppIdChanging(int value);
    partial void OnAppIdChanged();
    #endregion
		
		public DatabaseProcess()
		{
			this._DatabaseApplication = new EntitySet<DatabaseApplication>(new Action<DatabaseApplication>(this.attach_DatabaseApplication), new Action<DatabaseApplication>(this.detach_DatabaseApplication));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NVarChar(260) NOT NULL", CanBeNull=false)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Path", DbType="NVarChar(512) NOT NULL", CanBeNull=false)]
		public string Path
		{
			get
			{
				return this._Path;
			}
			set
			{
				if ((this._Path != value))
				{
					this.OnPathChanging(value);
					this.SendPropertyChanging();
					this._Path = value;
					this.SendPropertyChanged("Path");
					this.OnPathChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AppId", DbType="Int NOT NULL")]
		public int AppId
		{
			get
			{
				return this._AppId;
			}
			set
			{
				if ((this._AppId != value))
				{
					this.OnAppIdChanging(value);
					this.SendPropertyChanging();
					this._AppId = value;
					this.SendPropertyChanged("AppId");
					this.OnAppIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="DatabaseProcess_DatabaseApplication", Storage="_DatabaseApplication", ThisKey="AppId", OtherKey="Id")]
		public EntitySet<DatabaseApplication> DatabaseApplication
		{
			get
			{
				return this._DatabaseApplication;
			}
			set
			{
				this._DatabaseApplication.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_DatabaseApplication(DatabaseApplication entity)
		{
			this.SendPropertyChanging();
			entity.DatabaseProcess = this;
		}
		
		private void detach_DatabaseApplication(DatabaseApplication entity)
		{
			this.SendPropertyChanging();
			entity.DatabaseProcess = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.DatabaseApplication")]
	public partial class DatabaseApplication : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private System.Nullable<bool> _Favorite;
		
		private string _Title;
		
		private string _Memo;
		
		private EntityRef<DatabaseProcess> _DatabaseProcess;
		
		private EntityRef<DatabaseTimeline> _DatabaseTimeline;
		
		private EntityRef<DatabaseDate> _DatabaseDate;
		
    #region 拡張メソッドの定義
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnFavoriteChanging(System.Nullable<bool> value);
    partial void OnFavoriteChanged();
    partial void OnTitleChanging(string value);
    partial void OnTitleChanged();
    partial void OnMemoChanging(string value);
    partial void OnMemoChanged();
    #endregion
		
		public DatabaseApplication()
		{
			this._DatabaseProcess = default(EntityRef<DatabaseProcess>);
			this._DatabaseTimeline = default(EntityRef<DatabaseTimeline>);
			this._DatabaseDate = default(EntityRef<DatabaseDate>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					if (((this._DatabaseProcess.HasLoadedOrAssignedValue || this._DatabaseTimeline.HasLoadedOrAssignedValue) 
								|| this._DatabaseDate.HasLoadedOrAssignedValue))
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Favorite", DbType="Bit")]
		public System.Nullable<bool> Favorite
		{
			get
			{
				return this._Favorite;
			}
			set
			{
				if ((this._Favorite != value))
				{
					this.OnFavoriteChanging(value);
					this.SendPropertyChanging();
					this._Favorite = value;
					this.SendPropertyChanged("Favorite");
					this.OnFavoriteChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Title", DbType="NVarChar(MAX)")]
		public string Title
		{
			get
			{
				return this._Title;
			}
			set
			{
				if ((this._Title != value))
				{
					this.OnTitleChanging(value);
					this.SendPropertyChanging();
					this._Title = value;
					this.SendPropertyChanged("Title");
					this.OnTitleChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Memo", DbType="NVarChar(MAX)")]
		public string Memo
		{
			get
			{
				return this._Memo;
			}
			set
			{
				if ((this._Memo != value))
				{
					this.OnMemoChanging(value);
					this.SendPropertyChanging();
					this._Memo = value;
					this.SendPropertyChanged("Memo");
					this.OnMemoChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="DatabaseProcess_DatabaseApplication", Storage="_DatabaseProcess", ThisKey="Id", OtherKey="AppId", IsForeignKey=true)]
		public DatabaseProcess DatabaseProcess
		{
			get
			{
				return this._DatabaseProcess.Entity;
			}
			set
			{
				DatabaseProcess previousValue = this._DatabaseProcess.Entity;
				if (((previousValue != value) 
							|| (this._DatabaseProcess.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._DatabaseProcess.Entity = null;
						previousValue.DatabaseApplication.Remove(this);
					}
					this._DatabaseProcess.Entity = value;
					if ((value != null))
					{
						value.DatabaseApplication.Add(this);
						this._Id = value.AppId;
					}
					else
					{
						this._Id = default(int);
					}
					this.SendPropertyChanged("DatabaseProcess");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="DatabaseTimeline_DatabaseApplication", Storage="_DatabaseTimeline", ThisKey="Id", OtherKey="AppId", IsForeignKey=true)]
		public DatabaseTimeline DatabaseTimeline
		{
			get
			{
				return this._DatabaseTimeline.Entity;
			}
			set
			{
				DatabaseTimeline previousValue = this._DatabaseTimeline.Entity;
				if (((previousValue != value) 
							|| (this._DatabaseTimeline.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._DatabaseTimeline.Entity = null;
						previousValue.DatabaseApplication.Remove(this);
					}
					this._DatabaseTimeline.Entity = value;
					if ((value != null))
					{
						value.DatabaseApplication.Add(this);
						this._Id = value.AppId;
					}
					else
					{
						this._Id = default(int);
					}
					this.SendPropertyChanged("DatabaseTimeline");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="DatabaseDate_DatabaseApplication", Storage="_DatabaseDate", ThisKey="Id", OtherKey="AppId", IsForeignKey=true)]
		public DatabaseDate DatabaseDate
		{
			get
			{
				return this._DatabaseDate.Entity;
			}
			set
			{
				DatabaseDate previousValue = this._DatabaseDate.Entity;
				if (((previousValue != value) 
							|| (this._DatabaseDate.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._DatabaseDate.Entity = null;
						previousValue.DatabaseApplication.Remove(this);
					}
					this._DatabaseDate.Entity = value;
					if ((value != null))
					{
						value.DatabaseApplication.Add(this);
						this._Id = value.AppId;
					}
					else
					{
						this._Id = default(int);
					}
					this.SendPropertyChanged("DatabaseDate");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.DatabaseTimeline")]
	public partial class DatabaseTimeline : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private int _AppId;
		
		private string _UsageTime;
		
		private System.Nullable<System.DateTime> _Today;
		
		private System.Nullable<System.DateTime> _Now;
		
		private EntitySet<DatabaseApplication> _DatabaseApplication;
		
    #region 拡張メソッドの定義
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnAppIdChanging(int value);
    partial void OnAppIdChanged();
    partial void OnUsageTimeChanging(string value);
    partial void OnUsageTimeChanged();
    partial void OnTodayChanging(System.Nullable<System.DateTime> value);
    partial void OnTodayChanged();
    partial void OnNowChanging(System.Nullable<System.DateTime> value);
    partial void OnNowChanged();
    #endregion
		
		public DatabaseTimeline()
		{
			this._DatabaseApplication = new EntitySet<DatabaseApplication>(new Action<DatabaseApplication>(this.attach_DatabaseApplication), new Action<DatabaseApplication>(this.detach_DatabaseApplication));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AppId", DbType="Int NOT NULL")]
		public int AppId
		{
			get
			{
				return this._AppId;
			}
			set
			{
				if ((this._AppId != value))
				{
					this.OnAppIdChanging(value);
					this.SendPropertyChanging();
					this._AppId = value;
					this.SendPropertyChanged("AppId");
					this.OnAppIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_UsageTime", DbType="NVarChar(50)")]
		public string UsageTime
		{
			get
			{
				return this._UsageTime;
			}
			set
			{
				if ((this._UsageTime != value))
				{
					this.OnUsageTimeChanging(value);
					this.SendPropertyChanging();
					this._UsageTime = value;
					this.SendPropertyChanged("UsageTime");
					this.OnUsageTimeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Today", DbType="Date")]
		public System.Nullable<System.DateTime> Today
		{
			get
			{
				return this._Today;
			}
			set
			{
				if ((this._Today != value))
				{
					this.OnTodayChanging(value);
					this.SendPropertyChanging();
					this._Today = value;
					this.SendPropertyChanged("Today");
					this.OnTodayChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Now", DbType="DateTime")]
		public System.Nullable<System.DateTime> Now
		{
			get
			{
				return this._Now;
			}
			set
			{
				if ((this._Now != value))
				{
					this.OnNowChanging(value);
					this.SendPropertyChanging();
					this._Now = value;
					this.SendPropertyChanged("Now");
					this.OnNowChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="DatabaseTimeline_DatabaseApplication", Storage="_DatabaseApplication", ThisKey="AppId", OtherKey="Id")]
		public EntitySet<DatabaseApplication> DatabaseApplication
		{
			get
			{
				return this._DatabaseApplication;
			}
			set
			{
				this._DatabaseApplication.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_DatabaseApplication(DatabaseApplication entity)
		{
			this.SendPropertyChanging();
			entity.DatabaseTimeline = this;
		}
		
		private void detach_DatabaseApplication(DatabaseApplication entity)
		{
			this.SendPropertyChanging();
			entity.DatabaseTimeline = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.DatabaseDate")]
	public partial class DatabaseDate : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private string _UsageTime;
		
		private string _AlertTime;
		
		private System.Nullable<System.DateTime> _AddDate;
		
		private System.Nullable<System.DateTime> _LastDate;
		
		private int _AppId;
		
		private EntitySet<DatabaseApplication> _DatabaseApplication;
		
    #region 拡張メソッドの定義
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnUsageTimeChanging(string value);
    partial void OnUsageTimeChanged();
    partial void OnAlertTimeChanging(string value);
    partial void OnAlertTimeChanged();
    partial void OnAddDateChanging(System.Nullable<System.DateTime> value);
    partial void OnAddDateChanged();
    partial void OnLastDateChanging(System.Nullable<System.DateTime> value);
    partial void OnLastDateChanged();
    partial void OnAppIdChanging(int value);
    partial void OnAppIdChanged();
    #endregion
		
		public DatabaseDate()
		{
			this._DatabaseApplication = new EntitySet<DatabaseApplication>(new Action<DatabaseApplication>(this.attach_DatabaseApplication), new Action<DatabaseApplication>(this.detach_DatabaseApplication));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_UsageTime", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string UsageTime
		{
			get
			{
				return this._UsageTime;
			}
			set
			{
				if ((this._UsageTime != value))
				{
					this.OnUsageTimeChanging(value);
					this.SendPropertyChanging();
					this._UsageTime = value;
					this.SendPropertyChanged("UsageTime");
					this.OnUsageTimeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AlertTime", DbType="NVarChar(50)")]
		public string AlertTime
		{
			get
			{
				return this._AlertTime;
			}
			set
			{
				if ((this._AlertTime != value))
				{
					this.OnAlertTimeChanging(value);
					this.SendPropertyChanging();
					this._AlertTime = value;
					this.SendPropertyChanged("AlertTime");
					this.OnAlertTimeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AddDate", DbType="DateTime")]
		public System.Nullable<System.DateTime> AddDate
		{
			get
			{
				return this._AddDate;
			}
			set
			{
				if ((this._AddDate != value))
				{
					this.OnAddDateChanging(value);
					this.SendPropertyChanging();
					this._AddDate = value;
					this.SendPropertyChanged("AddDate");
					this.OnAddDateChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LastDate", DbType="DateTime")]
		public System.Nullable<System.DateTime> LastDate
		{
			get
			{
				return this._LastDate;
			}
			set
			{
				if ((this._LastDate != value))
				{
					this.OnLastDateChanging(value);
					this.SendPropertyChanging();
					this._LastDate = value;
					this.SendPropertyChanged("LastDate");
					this.OnLastDateChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AppId", DbType="Int NOT NULL")]
		public int AppId
		{
			get
			{
				return this._AppId;
			}
			set
			{
				if ((this._AppId != value))
				{
					this.OnAppIdChanging(value);
					this.SendPropertyChanging();
					this._AppId = value;
					this.SendPropertyChanged("AppId");
					this.OnAppIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="DatabaseDate_DatabaseApplication", Storage="_DatabaseApplication", ThisKey="AppId", OtherKey="Id")]
		public EntitySet<DatabaseApplication> DatabaseApplication
		{
			get
			{
				return this._DatabaseApplication;
			}
			set
			{
				this._DatabaseApplication.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_DatabaseApplication(DatabaseApplication entity)
		{
			this.SendPropertyChanging();
			entity.DatabaseDate = this;
		}
		
		private void detach_DatabaseApplication(DatabaseApplication entity)
		{
			this.SendPropertyChanging();
			entity.DatabaseDate = null;
		}
	}
}
#pragma warning restore 1591
