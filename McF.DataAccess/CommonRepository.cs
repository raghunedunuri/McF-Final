using McF.Contracts;
using McF.DataAcess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McF.DataAccess
{
    public class CommonRepository : ICommonRepository
    {
        private IDbHelper dbHelper = null;
        public CommonRepository(IDbHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }

        public List<DataSources> GetAllDataSources()
        {
            List<DataSources> lstDataSources = new List<DataSources>();
            using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_GetAllDataSources", CommandType.StoredProcedure))
            {
                IDataReader dataReader = dbHelper.ExecuteReader();
                while (dataReader.Read())
                {
                    DataSources ds = new DataSources()
                    {
                        DisplayName = dataReader["DisplayName"].ToString(),
                        DataSource = dataReader["DataSource"].ToString(),
                        RootSource = dataReader["RootSource"].ToString()
                    };
                    lstDataSources.Add(ds);
                }
            }
            return lstDataSources;
        }
        public void PopulateUser(UserLoginData userLoginData)
        {
            ProcessQuery($"Insert into USERS (UserName, LoginID, Password, IsActive) values ( '{userLoginData.UserName}','{userLoginData.UserId}','{userLoginData.Password}', 1)");
        }

        public bool ValidateUser(string user, string passWord)
        {
            DataSet userData = ProcessDataQuery($"Select * from users where UserName = '{user}' and Password = '{passWord}'");

            if (userData != null && userData.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            return false;
        }
        public DSFormatedData GetFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null)
        {
            DSFormatedData dataSourceData = new DSFormatedData();
            Dictionary<string, HeaderFields> dictHeader = new Dictionary<string, HeaderFields>();

            using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_GET_SUGAR_DATA", CommandType.StoredProcedure))
            {
                dbHelper.AddParameter(dbCommand, "@INDEX", Index);
                if (From.HasValue)
                    dbHelper.AddParameter(dbCommand, "@From", From);
                if (To.HasValue)
                    dbHelper.AddParameter(dbCommand, "@To", To);

                DataSet ds = dbHelper.ExecuteDataSet();
                dbHelper.CloseConnection();
                DataTable Headers = ds.Tables[1];
                foreach (DataRow dr in Headers.Rows)
                {
                    HeaderFields header = new HeaderFields()
                    {
                        Name = dr["FIELD_NAME"].ToString(),
                        DisplayName = dr["DISPLAY_NAME"].ToString(),
                        Unit = dr["FIELD_UNITS"].ToString()
                    };
                    dictHeader.Add(header.Name, header);
                }
                FormatedData formatedData = new FormatedData();
                DataTable FormatedTable = ds.Tables[0];

                foreach (DataColumn dc in FormatedTable.Columns)
                {
                    string columnName = dc.ColumnName;
                    if (dictHeader.ContainsKey(columnName))
                    {
                        formatedData.Headers.Add(new HeaderFields()
                        {
                            Name = dictHeader[columnName].Name,
                            DisplayName = dictHeader[columnName].DisplayName,
                            ReadOnly = false,
                            Unit = dictHeader[columnName].Unit
                        });
                    }
                    else
                    {
                        formatedData.Headers.Add(new HeaderFields()
                        {
                            Name = columnName,
                            DisplayName = columnName,
                            ReadOnly = true
                        });
                    }
                }
                DataTable FormatData = ds.Tables[0];
                foreach (DataRow dr in FormatData.Rows)
                {
                    List<string> lstValue = new List<string>();
                    foreach (HeaderFields hf in formatedData.Headers)
                    {
                        lstValue.Add(dr[hf.Name].ToString());
                    }
                    formatedData.Values.Add(lstValue);
                }
                formatedData.CommodityData = new RootData()
                {
                    Name = "Sugar",
                    TableName = "SUGAR_SUPPLY_DEMAND_DATA,SUGAR_BUYER_DATA"
                };
                dataSourceData.DictFormatedData.Add("SUGAR", formatedData);
            }
            return dataSourceData;
        }

        public Dictionary<string, string> GetDataSources()
        {
            Dictionary<string, string> dataSources = new Dictionary<string, string>();

            using (IDbCommand dbCommand = dbHelper.CreateCommand("SELECT DATASOURCENAME, DataSoruceTable FROM FREEFlowTables", CommandType.Text))
            {
                IDataReader dr = dbHelper.ExecuteReader();
                while (dr.Read())
                {
                    dataSources.Add(dr["DATASOURCENAME"].ToString(), dr["DataSoruceTable"].ToString());
                }
            }
            return dataSources;
        }

        public List<string> GetDataSourceFields(string tableName)
        {
            List<string> dataFields = new List<string>();

            using (IDbCommand dbCommand = dbHelper.CreateCommand($"SELECT DisplayName FROM FREEFLOW_CONFIG WHERE TABLENAME = '{tableName}'", CommandType.Text))
            {
                IDataReader dr = dbHelper.ExecuteReader();
                while (dr.Read())
                {
                    dataFields.Add(dr["DisplayName"].ToString());
                }
            }
            return dataFields;
        }

        public DSFormatedData GetWasdeWorldFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null)
        {
            DSFormatedData dataSourceData = new DSFormatedData();
            Dictionary<string, HeaderFields> dictHeader = new Dictionary<string, HeaderFields>();

            Dictionary<string, RootData> dictRoot = new Dictionary<string, RootData>();
            Dictionary<string, FormatedData> dictData = new Dictionary<string, FormatedData>();

            Dictionary<string, RowData> dictRowData = new Dictionary<string, RowData>();
            Dictionary<string, List<RowData>> dictCommData = new Dictionary<string, List<RowData>>();

            using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_GET_WASDE_WORLD_DATA", CommandType.StoredProcedure))
            {
                dbHelper.AddParameter(dbCommand, "@INDEX", Index);
                if (From.HasValue)
                    dbHelper.AddParameter(dbCommand, "@From", From);
                if (To.HasValue)
                    dbHelper.AddParameter(dbCommand, "@To", To);

                DataSet ds = dbHelper.ExecuteDataSet();
                dbHelper.CloseConnection();

                DataTable Commodiites = ds.Tables[0];

                foreach (DataRow dr in Commodiites.Rows)
                {
                    RootData rootData = new RootData();
                    rootData.Name = dr["Commodity_Name"].ToString();
                    rootData.Unit = dr["Units"].ToString();
                    rootData.Properties.Add("Page", dr["Page"].ToString());
                    dictRoot.Add(rootData.Name, rootData);
                }

                DataTable YearlyTable = ds.Tables[1];

                foreach (DataRow dr in YearlyTable.Rows)
                {
                    string ComName = dr["Commodity_Name"].ToString();
                    string DataDate = Convert.ToDateTime(dr["Data_Date"]).ToShortDateString();
                    string DataYear = dr["Data_Year"].ToString();
                    string DataMonth = dr["Data_Month"].ToString();
                    string Region = dr["Region"].ToString();
                    string Field = dr["Field"].ToString();
                    string value = dr["DataValue"].ToString();
                    string key = $"{ComName}{DataDate}{Region}{DataYear}{DataMonth}";

                    if (dictRowData.ContainsKey(key))
                    {
                        dictRowData[key].Data.Add(Field, value);
                    }
                    else
                    {
                        dictRowData.Add(key, new RowData());
                        dictRowData[key].Commodity = ComName;
                        dictRowData[key].Data.Add("Commodity_Name", ComName);
                        dictRowData[key].Data.Add("Data_Date", DataDate);
                        dictRowData[key].Data.Add("Data_Year", DataYear);
                        dictRowData[key].Data.Add("Data_Month", DataMonth);
                        dictRowData[key].Data.Add("Region", Region);
                        dictRowData[key].Data.Add(Field, value);
                    }
                }

                foreach (KeyValuePair<string, RowData> kv in dictRowData)
                {
                    RootData commData = dictRoot[kv.Value.Commodity];
                    RootData commDataYear = new RootData();
                    commDataYear.Name = $"{commData.Name} - Current Estimates";
                    commDataYear.TableName = "WASDE_WORLD_YEARLY_DATA";
                    commDataYear.Unit = commData.Unit;
                    commDataYear.Properties = commData.Properties;
                    List<string> lstRow = new List<string>();

                    if (dataSourceData.DictFormatedData.ContainsKey(commDataYear.Name))
                    {
                        foreach (KeyValuePair<string, string> rv in kv.Value.Data)
                        {
                            lstRow.Add(rv.Value);
                        }
                        dataSourceData.DictFormatedData[commDataYear.Name].Values.Add(lstRow);
                    }
                    else
                    {
                        FormatedData formatData = new FormatedData();

                        foreach (KeyValuePair<string, string> rv in kv.Value.Data)
                        {
                            bool bReadOnly = false;
                            if (rv.Key == "Commodity_Name" || rv.Key == "Data_Date" ||
                                rv.Key == "Region" || rv.Key == "Data_Year" || rv.Key == "Data_Month")
                            {
                                bReadOnly = true;
                            }
                            string key = $"{commData.Name}-{rv.Key}";
                            formatData.Headers.Add(new HeaderFields()
                            {
                                Name = rv.Key,
                                DisplayName = rv.Key,
                                Unit = String.Empty,
                                ReadOnly = bReadOnly
                            });
                            lstRow.Add(rv.Value);
                        }
                        formatData.Values.Add(lstRow);
                        formatData.CommodityData = commDataYear;
                        dataSourceData.DictFormatedData.Add(commDataYear.Name, formatData);
                    }
                }
                dictRowData.Clear();

                DataTable MonthlyData = ds.Tables[2];
                foreach (DataRow dr in MonthlyData.Rows)
                {
                    string ComName = dr["Commodity_Name"].ToString();
                    string DataDate = Convert.ToDateTime(dr["Data_Date"]).ToShortDateString();
                    string DataYear = dr["Data_Year"].ToString();
                    string DataMonth = dr["Data_Month"].ToString();
                    string Estonth = dr["Est_Month"].ToString();
                    string Region = dr["Region"].ToString();
                    string Field = dr["Field"].ToString();
                    string value = dr["DataValue"].ToString();
                    string key = $"{ComName}{DataDate}{Region}{DataYear}{DataMonth}{Estonth}";

                    if (dictRowData.ContainsKey(key))
                    {
                        dictRowData[key].Data.Add(Field, value);
                    }
                    else
                    {
                        dictRowData.Add(key, new RowData());
                        dictRowData[key].Commodity = ComName;
                        dictRowData[key].Data.Add("Commodity_Name", ComName);
                        dictRowData[key].Data.Add("Data_Date", DataDate);
                        dictRowData[key].Data.Add("Data_Year", DataYear);
                        dictRowData[key].Data.Add("Data_Month", DataMonth);
                        dictRowData[key].Data.Add("Est_Month", Estonth);
                        dictRowData[key].Data.Add("Region", Region);
                        dictRowData[key].Data.Add(Field, value);
                    }
                }

                foreach (KeyValuePair<string, RowData> kv in dictRowData)
                {
                    RootData commData = dictRoot[kv.Value.Commodity];
                    RootData commDataYear = new RootData();
                    commDataYear.Name = $"{commData.Name} - Proj Estimates";
                    commDataYear.Unit = commData.Unit;
                    commDataYear.TableName = "WASDE_DOMESTIC_MONTHLY_DATA";
                    commDataYear.Properties = commData.Properties;
                    List<string> lstRow = new List<string>();

                    if (dataSourceData.DictFormatedData.ContainsKey(commDataYear.Name))
                    {
                        foreach (KeyValuePair<string, string> rv in kv.Value.Data)
                        {
                            lstRow.Add(rv.Value);
                        }
                        dataSourceData.DictFormatedData[commDataYear.Name].Values.Add(lstRow);
                    }
                    else
                    {
                        FormatedData formatData = new FormatedData();

                        foreach (KeyValuePair<string, string> rv in kv.Value.Data)
                        {
                            bool bReadOnly = false;
                            if (rv.Key == "Commodity_Name" || rv.Key == "Data_Date" || rv.Key == "Est_Month" ||
                                rv.Key == "Region" || rv.Key == "Data_Year" || rv.Key == "Data_Month")
                            {
                                bReadOnly = true;
                            }
                            string key = $"{commData.Name}-{rv.Key}";
                            formatData.Headers.Add(new HeaderFields()
                            {
                                Name = rv.Key,
                                DisplayName = rv.Key,
                                Unit = String.Empty,
                                ReadOnly = bReadOnly
                            });
                            lstRow.Add(rv.Value);
                        }
                        formatData.Values.Add(lstRow);
                        formatData.CommodityData = commDataYear;
                        dataSourceData.DictFormatedData.Add(commDataYear.Name, formatData);
                    }
                }
            }
            return dataSourceData;
        }

        public DSFormatedData GetWasdeWorldFormatedData(List<string> symbols, int Index = 0, DateTime? From = null, DateTime? To = null)
        {
            DSFormatedData dataSourceData = new DSFormatedData();
            Dictionary<string, HeaderFields> dictHeader = new Dictionary<string, HeaderFields>();

            Dictionary<string, RootData> dictRoot = new Dictionary<string, RootData>();
            Dictionary<string, FormatedData> dictData = new Dictionary<string, FormatedData>();

            Dictionary<string, RowData> dictRowData = new Dictionary<string, RowData>();
            Dictionary<string, List<RowData>> dictCommData = new Dictionary<string, List<RowData>>();

            using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_GET_WW_UPDATED", CommandType.StoredProcedure))
            {
                dbHelper.AddParameter(dbCommand, "@SymbolTable", CreateTable(symbols));
                dbHelper.AddParameter(dbCommand, "@INDEX", Index);
                if (From.HasValue)
                    dbHelper.AddParameter(dbCommand, "@From", From);
                if (To.HasValue)
                    dbHelper.AddParameter(dbCommand, "@To", To);

                DataSet ds = dbHelper.ExecuteDataSet();
                dbHelper.CloseConnection();

                DataTable Commodiites = ds.Tables[0];

                foreach (DataRow dr in Commodiites.Rows)
                {
                    RootData rootData = new RootData();
                    rootData.Name = dr["Commodity_Name"].ToString();
                    rootData.Unit = dr["Units"].ToString();
                    rootData.Properties.Add("Page", dr["Page"].ToString());
                    dictRoot.Add(rootData.Name, rootData);
                }

                DataTable YearlyTable = ds.Tables[1];

                foreach (DataRow dr in YearlyTable.Rows)
                {
                    string ComName = dr["Commodity_Name"].ToString();
                    string DataDate = Convert.ToDateTime(dr["Data_Date"]).ToShortDateString();
                    string DataYear = dr["Data_Year"].ToString();
                    string DataMonth = dr["Data_Month"].ToString();
                    string Region = dr["Region"].ToString();
                    string Field = dr["Field"].ToString();
                    string value = dr["DataValue"].ToString();
                    string key = $"{ComName}{DataDate}{Region}{DataYear}{DataMonth}";

                    if (dictRowData.ContainsKey(key))
                    {
                        dictRowData[key].Data.Add(Field, value);
                    }
                    else
                    {
                        dictRowData.Add(key, new RowData());
                        dictRowData[key].Commodity = ComName;
                        dictRowData[key].Data.Add("Commodity_Name", ComName);
                        dictRowData[key].Data.Add("Data_Date", DataDate);
                        dictRowData[key].Data.Add("Data_Year", DataYear);
                        dictRowData[key].Data.Add("Data_Month", DataMonth);
                        dictRowData[key].Data.Add("Region", Region);
                        dictRowData[key].Data.Add(Field, value);
                    }
                }

                foreach (KeyValuePair<string, RowData> kv in dictRowData)
                {
                    RootData commData = dictRoot[kv.Value.Commodity];
                    RootData commDataYear = new RootData();
                    commDataYear.Name = $"{commData.Name} - Current Estimates";
                    commDataYear.TableName = "WASDE_WORLD_YEARLY_DATA";
                    commDataYear.Unit = commData.Unit;
                    commDataYear.Properties = commData.Properties;
                    List<string> lstRow = new List<string>();

                    if (dataSourceData.DictFormatedData.ContainsKey(commDataYear.Name))
                    {
                        foreach (KeyValuePair<string, string> rv in kv.Value.Data)
                        {
                            lstRow.Add(rv.Value);
                        }
                        dataSourceData.DictFormatedData[commDataYear.Name].Values.Add(lstRow);
                    }
                    else
                    {
                        FormatedData formatData = new FormatedData();

                        foreach (KeyValuePair<string, string> rv in kv.Value.Data)
                        {
                            bool bReadOnly = false;
                            if (rv.Key == "Commodity_Name" || rv.Key == "Data_Date" ||
                                rv.Key == "Region" || rv.Key == "Data_Year" || rv.Key == "Data_Month")
                            {
                                bReadOnly = true;
                            }
                            string key = $"{commData.Name}-{rv.Key}";
                            formatData.Headers.Add(new HeaderFields()
                            {
                                Name = rv.Key,
                                DisplayName = rv.Key,
                                Unit = String.Empty,
                                ReadOnly = bReadOnly
                            });
                            lstRow.Add(rv.Value);
                        }
                        formatData.Values.Add(lstRow);
                        formatData.CommodityData = commDataYear;
                        dataSourceData.DictFormatedData.Add(commDataYear.Name, formatData);
                    }
                }
                dictRowData.Clear();

                DataTable MonthlyData = ds.Tables[2];
                foreach (DataRow dr in MonthlyData.Rows)
                {
                    string ComName = dr["Commodity_Name"].ToString();
                    string DataDate = Convert.ToDateTime(dr["Data_Date"]).ToShortDateString();
                    string DataYear = dr["Data_Year"].ToString();
                    string DataMonth = dr["Data_Month"].ToString();
                    string Estonth = dr["Est_Month"].ToString();
                    string Region = dr["Region"].ToString();
                    string Field = dr["Field"].ToString();
                    string value = dr["DataValue"].ToString();
                    string key = $"{ComName}{DataDate}{Region}{DataYear}{DataMonth}{Estonth}";

                    if (dictRowData.ContainsKey(key))
                    {
                        dictRowData[key].Data.Add(Field, value);
                    }
                    else
                    {
                        dictRowData.Add(key, new RowData());
                        dictRowData[key].Commodity = ComName;
                        dictRowData[key].Data.Add("Commodity_Name", ComName);
                        dictRowData[key].Data.Add("Data_Date", DataDate);
                        dictRowData[key].Data.Add("Data_Year", DataYear);
                        dictRowData[key].Data.Add("Data_Month", DataMonth);
                        dictRowData[key].Data.Add("Est_Month", Estonth);
                        dictRowData[key].Data.Add("Region", Region);
                        dictRowData[key].Data.Add(Field, value);
                    }
                }

                foreach (KeyValuePair<string, RowData> kv in dictRowData)
                {
                    RootData commData = dictRoot[kv.Value.Commodity];
                    RootData commDataYear = new RootData();
                    commDataYear.Name = $"{commData.Name} - Proj Estimates";
                    commDataYear.Unit = commData.Unit;
                    commDataYear.TableName = "WASDE_DOMESTIC_MONTHLY_DATA";
                    commDataYear.Properties = commData.Properties;
                    List<string> lstRow = new List<string>();

                    if (dataSourceData.DictFormatedData.ContainsKey(commDataYear.Name))
                    {
                        foreach (KeyValuePair<string, string> rv in kv.Value.Data)
                        {
                            lstRow.Add(rv.Value);
                        }
                        dataSourceData.DictFormatedData[commDataYear.Name].Values.Add(lstRow);
                    }
                    else
                    {
                        FormatedData formatData = new FormatedData();

                        foreach (KeyValuePair<string, string> rv in kv.Value.Data)
                        {
                            bool bReadOnly = false;
                            if (rv.Key == "Commodity_Name" || rv.Key == "Data_Date" || rv.Key == "Est_Month" ||
                                rv.Key == "Region" || rv.Key == "Data_Year" || rv.Key == "Data_Month")
                            {
                                bReadOnly = true;
                            }
                            string key = $"{commData.Name}-{rv.Key}";
                            formatData.Headers.Add(new HeaderFields()
                            {
                                Name = rv.Key,
                                DisplayName = rv.Key,
                                Unit = String.Empty,
                                ReadOnly = bReadOnly
                            });
                            lstRow.Add(rv.Value);
                        }
                        formatData.Values.Add(lstRow);
                        formatData.CommodityData = commDataYear;
                        dataSourceData.DictFormatedData.Add(commDataYear.Name, formatData);
                    }
                }
            }
            return dataSourceData;
        }
        public DSFormatedData GetFOFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null)
        {
            DSFormatedData dataSourceData = new DSFormatedData();
            Dictionary<string, FOField> dictFields = new Dictionary<string, FOField>();

            Dictionary<string, HeaderFields> dictHeader = new Dictionary<string, HeaderFields>();

            Dictionary<string, RootData> dictRoot = new Dictionary<string, RootData>();
            Dictionary<string, FormatedData> dictData = new Dictionary<string, FormatedData>();

            Dictionary<string, RowData> dictRowData = new Dictionary<string, RowData>();
            Dictionary<string, List<RowData>> dictCommData = new Dictionary<string, List<RowData>>();

            Dictionary<string, string> dictFieldData = new Dictionary<string, string>();
            using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_GET_FO_LAST_UPDATED", CommandType.StoredProcedure))
            {
                dbHelper.AddParameter(dbCommand, "@INDEX", Index);
                if (From.HasValue)
                    dbHelper.AddParameter(dbCommand, "@From", From);
                if (To.HasValue)
                    dbHelper.AddParameter(dbCommand, "@To", To);

                DataSet ds = dbHelper.ExecuteDataSet();
                dbHelper.CloseConnection();

                DataTable Fields = ds.Tables[1];

                foreach (DataRow dr in Fields.Rows)
                {
                    FOField field = new FOField();
                    string Field = dr["Field"].ToString();
                    string category = dr["Category"].ToString();
                    string key = $"{category}--{Field}";
                    if (!dictFields.ContainsKey(key))
                    {
                        dictFields.Add(key, new FOField()
                        {
                            Field = Field,
                            DisplayName = Field,
                            Category = category,
                            Unit = dr["Unit"].ToString()
                        });
                    }
                }

                DataTable FOData = ds.Tables[0];

                foreach (DataRow dr in FOData.Rows)
                {
                    string ComName = dr["Commodity_Name"].ToString();
                    string Category = dr["Category"].ToString();
                    string ReportDate = dr["Report_Date"].ToString();
                    string MonthEndDate = dr["Month_EndDate"].ToString();
                    string Field = dr["Field"].ToString();
                    string value = dr["DataValue"].ToString();
                    string key = $"{ComName}{Category}{ReportDate}{MonthEndDate}";

                    if (dictRowData.ContainsKey(key))
                    {
                        dictRowData[key].Data.Add(Field, value);
                    }
                    else
                    {
                        dictRowData.Add(key, new RowData());
                        dictRowData[key].Commodity = ComName;
                        dictRowData[key].Category = Category;
                        dictRowData[key].Data.Add("CommodityName", ComName);
                        dictRowData[key].Data.Add("Category", Category);
                        dictRowData[key].Data.Add("ReportDate", ReportDate);
                        dictRowData[key].Data.Add("MonthEndDate", MonthEndDate);
                        dictRowData[key].Data.Add(Field, value);
                    }
                }

                foreach (KeyValuePair<string, RowData> kv in dictRowData)
                {
                    RootData commDataYear = new RootData();
                    commDataYear.Name = $"{kv.Value.Commodity}";
                    commDataYear.TableName = "FATSANDOIL_DATA";
                    List<string> lstRow = new List<string>();

                    if (dataSourceData.DictFormatedData.ContainsKey(commDataYear.Name))
                    {
                        foreach (KeyValuePair<string, string> rv in kv.Value.Data)
                        {
                            lstRow.Add(rv.Value);
                        }
                        dataSourceData.DictFormatedData[commDataYear.Name].Values.Add(lstRow);
                    }
                    else
                    {
                        FormatedData formatData = new FormatedData();

                        foreach (KeyValuePair<string, string> rv in kv.Value.Data)
                        {
                            bool bReadOnly = false;
                            if (rv.Key == "CommodityName" || rv.Key == "Category" ||
                                rv.Key == "ReportDate" || rv.Key == "MonthEndDate")
                            {
                                bReadOnly = true;
                            }
                            string key = $"{kv.Value.Category}--{rv.Key}";
                            formatData.Headers.Add(new HeaderFields()
                            {
                                Name = rv.Key,
                                DisplayName = rv.Key,
                                Unit = bReadOnly ? String.Empty : dictFields[key].Unit,
                                ReadOnly = bReadOnly
                            });
                            lstRow.Add(rv.Value);
                        }
                        formatData.Values.Add(lstRow);
                        formatData.CommodityData = commDataYear;
                        dataSourceData.DictFormatedData.Add(commDataYear.Name, formatData);
                    }
                }
                dictRowData.Clear();

                DataTable FORData = ds.Tables[2];

                foreach (DataRow dr in FORData.Rows)
                {
                    string ComName = dr["Commodity_Name"].ToString();
                    string Category = dr["Category"].ToString();
                    string Region = dr["Region"].ToString();
                    string ReportDate = dr["Report_Date"].ToString();
                    string MonthEndDate = dr["Month_EndDate"].ToString();
                    string Field = dr["Field"].ToString();
                    string value = dr["DataValue"].ToString();
                    string key = $"{ComName}{Category}{Region}{ReportDate}{MonthEndDate}";

                    if (dictRowData.ContainsKey(key))
                    {
                        dictRowData[key].Data.Add(Field, value);
                    }
                    else
                    {
                        dictRowData.Add(key, new RowData());
                        dictRowData[key].Commodity = ComName;
                        dictRowData[key].Category = Category;
                        dictRowData[key].Data.Add("CommodityName", ComName);
                        dictRowData[key].Data.Add("Category", Category);
                        dictRowData[key].Data.Add("ReportDate", ReportDate);
                        dictRowData[key].Data.Add("Region", Region);
                        dictRowData[key].Data.Add("MonthEndDate", MonthEndDate);
                        dictRowData[key].Data.Add(Field, value);
                    }
                }

                foreach (KeyValuePair<string, RowData> kv in dictRowData)
                {
                    RootData commDataYear = new RootData();
                    commDataYear.Name = $"{kv.Value.Commodity} - REGIONAL DATA";
                    commDataYear.TableName = "FATSANDOIL_REGIONAL_DATA";
                    List<string> lstRow = new List<string>();

                    if (dataSourceData.DictFormatedData.ContainsKey(commDataYear.Name))
                    {
                        foreach (KeyValuePair<string, string> rv in kv.Value.Data)
                        {
                            lstRow.Add(rv.Value);
                        }
                        dataSourceData.DictFormatedData[commDataYear.Name].Values.Add(lstRow);
                    }
                    else
                    {
                        FormatedData formatData = new FormatedData();

                        foreach (KeyValuePair<string, string> rv in kv.Value.Data)
                        {
                            bool bReadOnly = false;
                            if (rv.Key == "CommodityName" || rv.Key == "Category" || rv.Key == "Region" ||
                                rv.Key == "ReportDate" || rv.Key == "MonthEndDate")
                            {
                                bReadOnly = true;
                            }
                            string key = $"{kv.Value.Category}--{rv.Key}";
                            formatData.Headers.Add(new HeaderFields()
                            {
                                Name = rv.Key,
                                DisplayName = rv.Key,
                                Unit = bReadOnly ? String.Empty : dictFields[key].Unit,
                                ReadOnly = bReadOnly
                            });
                            lstRow.Add(rv.Value);
                        }
                        formatData.Values.Add(lstRow);
                        formatData.CommodityData = commDataYear;
                        dataSourceData.DictFormatedData.Add(commDataYear.Name, formatData);
                    }
                }
            }
            return dataSourceData;
        }
        public DSFormatedData GetWasdeDomesticFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null)
        {
            DSFormatedData dataSourceData = new DSFormatedData();
            Dictionary<string, HeaderFields> dictHeader = new Dictionary<string, HeaderFields>();

            Dictionary<string, RootData> dictRoot = new Dictionary<string, RootData>();
            Dictionary<string, FormatedData> dictData = new Dictionary<string, FormatedData>();

            Dictionary<string, RowData> dictRowData = new Dictionary<string, RowData>();
            Dictionary<string, List<RowData>> dictCommData = new Dictionary<string, List<RowData>>();

            Dictionary<string, string> dictFieldData = new Dictionary<string, string>();
            using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_GET_WASDE_DOMESTIC_DATA", CommandType.StoredProcedure))
            {
                dbHelper.AddParameter(dbCommand, "@INDEX", Index);
                if (From.HasValue)
                    dbHelper.AddParameter(dbCommand, "@From", From);
                if (To.HasValue)
                    dbHelper.AddParameter(dbCommand, "@To", To);

                DataSet ds = dbHelper.ExecuteDataSet();
                dbHelper.CloseConnection();

                DataTable Commodiites = ds.Tables[0];

                foreach (DataRow dr in Commodiites.Rows)
                {
                    RootData rootData = new RootData();
                    rootData.Name = dr["Commodity_Name"].ToString();
                    rootData.Unit = String.Empty;
                    rootData.Properties.Add("Page", dr["Page"].ToString());
                    dictRoot.Add(rootData.Name, rootData);
                }

                DataTable Fields = ds.Tables[1];

                foreach (DataRow dr in Fields.Rows)
                {
                    string commName = dr["Commodity_Name"].ToString();
                    string Field = dr["Field"].ToString();
                    string key = $"{commName}-{Field}";

                    if (!dictFieldData.ContainsKey(key))
                        dictFieldData[key] = dr["Unit"].ToString();
                }

                DataTable YearlyTable = ds.Tables[2];

                foreach (DataRow dr in YearlyTable.Rows)
                {
                    string ComName = dr["Commodity_Name"].ToString();
                    string DataDate = Convert.ToDateTime(dr["Data_Date"]).ToShortDateString();
                    string DataYear = dr["Data_Year"].ToString();
                    string DataMonth = dr["Data_Month"].ToString();
                    string Field = dr["Field"].ToString();
                    string value = dr["DataValue"].ToString();
                    string key = $"{ComName}{DataDate}{DataYear}{DataMonth}";

                    if (dictRowData.ContainsKey(key))
                    {
                        dictRowData[key].Data.Add(Field, value);
                    }
                    else
                    {
                        dictRowData.Add(key, new RowData());
                        dictRowData[key].Commodity = ComName;
                        dictRowData[key].Data.Add("Commodity_Name", ComName);
                        dictRowData[key].Data.Add("Data_Date", DataDate);
                        dictRowData[key].Data.Add("Data_Year", DataYear);
                        dictRowData[key].Data.Add("Data_Month", DataMonth);
                        dictRowData[key].Data.Add(Field, value);
                    }
                }

                foreach (KeyValuePair<string, RowData> kv in dictRowData)
                {
                    RootData commData = dictRoot[kv.Value.Commodity];
                    RootData commDataYear = new RootData();
                    commDataYear.Name = $"{commData.Name} - Current Estimates";
                    commDataYear.TableName = $"WASDE_DOMESTIC_YEARLY_DATA";
                    commDataYear.Unit = commData.Unit;
                    commDataYear.Properties = commData.Properties;
                    List<string> lstRow = new List<string>();

                    if (dataSourceData.DictFormatedData.ContainsKey(commDataYear.Name))
                    {
                        foreach (KeyValuePair<string, string> rv in kv.Value.Data)
                        {
                            lstRow.Add(rv.Value);
                        }
                        dataSourceData.DictFormatedData[commDataYear.Name].Values.Add(lstRow);
                    }
                    else
                    {
                        FormatedData formatData = new FormatedData();

                        foreach (KeyValuePair<string, string> rv in kv.Value.Data)
                        {
                            bool bReadOnly = false;
                            if (rv.Key == "Commodity_Name" || rv.Key == "Data_Date" ||
                                rv.Key == "Data_Year" || rv.Key == "Data_Month")
                            {
                                bReadOnly = true;
                            }
                            string key = $"{commData.Name}-{rv.Key}";
                            formatData.Headers.Add(new HeaderFields()
                            {
                                Name = rv.Key,
                                DisplayName = rv.Key,
                                Unit = bReadOnly ? String.Empty : dictFieldData[key],
                                ReadOnly = bReadOnly
                            });
                            lstRow.Add(rv.Value);
                        }
                        formatData.Values.Add(lstRow);
                        formatData.CommodityData = commDataYear;
                        dataSourceData.DictFormatedData.Add(commDataYear.Name, formatData);
                    }
                }
                dictRowData.Clear();

                DataTable MonthlyData = ds.Tables[3];
                foreach (DataRow dr in MonthlyData.Rows)
                {
                    string ComName = dr["Commodity_Name"].ToString();
                    string DataDate = Convert.ToDateTime(dr["Data_Date"]).ToShortDateString();
                    string DataYear = dr["Data_Year"].ToString();
                    string DataMonth = dr["Data_Month"].ToString();
                    string Estonth = dr["Est_Month"].ToString();
                    string Field = dr["Field"].ToString();
                    string value = dr["DataValue"].ToString();
                    string key = $"{ComName}{DataDate}{DataYear}{DataMonth}{Estonth}";

                    if (dictRowData.ContainsKey(key))
                    {
                        dictRowData[key].Data.Add(Field, value);
                    }
                    else
                    {
                        dictRowData.Add(key, new RowData());
                        dictRowData[key].Commodity = ComName;
                        dictRowData[key].Data.Add("Commodity_Name", ComName);
                        dictRowData[key].Data.Add("Data_Date", DataDate);
                        dictRowData[key].Data.Add("Data_Year", DataYear);
                        dictRowData[key].Data.Add("Data_Month", DataMonth);
                        dictRowData[key].Data.Add("Est_Month", Estonth);
                        dictRowData[key].Data.Add(Field, value);
                    }
                }

                foreach (KeyValuePair<string, RowData> kv in dictRowData)
                {
                    RootData commData = dictRoot[kv.Value.Commodity];
                    RootData commDataYear = new RootData();
                    commDataYear.Name = $"{commData.Name}- Proj Estimates";
                    commDataYear.TableName = $"WASDE_DOMESTIC_MONTHLY_DATA";
                    commDataYear.Unit = commData.Unit;
                    commDataYear.Properties = commData.Properties;
                    List<string> lstRow = new List<string>();

                    if (dataSourceData.DictFormatedData.ContainsKey(commDataYear.Name))
                    {
                        foreach (KeyValuePair<string, string> rv in kv.Value.Data)
                        {
                            lstRow.Add(rv.Value);
                        }
                        dataSourceData.DictFormatedData[commDataYear.Name].Values.Add(lstRow);
                    }
                    else
                    {
                        FormatedData formatData = new FormatedData();

                        foreach (KeyValuePair<string, string> rv in kv.Value.Data)
                        {
                            bool bReadOnly = false;
                            if (rv.Key == "Commodity_Name" || rv.Key == "Data_Date" ||
                               rv.Key == "Est_Month" || rv.Key == "Data_Year" || rv.Key == "Data_Month")
                            {
                                bReadOnly = true;
                            }
                            string key = $"{commData.Name}-{rv.Key}";
                            formatData.Headers.Add(new HeaderFields()
                            {
                                Name = rv.Key,
                                DisplayName = rv.Key,
                                Unit = bReadOnly ? String.Empty : dictFieldData[key],
                                ReadOnly = bReadOnly
                            });
                            lstRow.Add(rv.Value);
                        }
                        formatData.Values.Add(lstRow);
                        formatData.CommodityData = commDataYear;
                        dataSourceData.DictFormatedData.Add(commDataYear.Name, formatData);
                    }
                }
            }
            return dataSourceData;
        }

        public DSFormatedData GetWasdeDomesticFormatedData(List<string> symbols, int Index = 0, DateTime? From = null, DateTime? To = null)
        {
            DSFormatedData dataSourceData = new DSFormatedData();
            Dictionary<string, HeaderFields> dictHeader = new Dictionary<string, HeaderFields>();

            Dictionary<string, RootData> dictRoot = new Dictionary<string, RootData>();
            Dictionary<string, FormatedData> dictData = new Dictionary<string, FormatedData>();

            Dictionary<string, RowData> dictRowData = new Dictionary<string, RowData>();
            Dictionary<string, List<RowData>> dictCommData = new Dictionary<string, List<RowData>>();

            Dictionary<string, string> dictFieldData = new Dictionary<string, string>();
            using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_GET_WD_UPDATED", CommandType.StoredProcedure))
            {
                DataTable dt = CreateTable(symbols);
                dbHelper.AddParameter(dbCommand, "@SymbolTable", dt);
                dbHelper.AddParameter(dbCommand, "@INDEX", Index);
                if (From.HasValue)
                    dbHelper.AddParameter(dbCommand, "@From", From);
                if (To.HasValue)
                    dbHelper.AddParameter(dbCommand, "@To", To);

                DataSet ds = dbHelper.ExecuteDataSet();
                dbHelper.CloseConnection();

                DataTable Commodiites = ds.Tables[0];

                foreach (DataRow dr in Commodiites.Rows)
                {
                    RootData rootData = new RootData();
                    rootData.Name = dr["Commodity_Name"].ToString();
                    rootData.Unit = String.Empty;
                    rootData.Properties.Add("Page", dr["Page"].ToString());
                    dictRoot.Add(rootData.Name, rootData);
                }

                DataTable Fields = ds.Tables[1];

                foreach (DataRow dr in Fields.Rows)
                {
                    string commName = dr["Commodity_Name"].ToString();
                    string Field = dr["Field"].ToString();
                    string key = $"{commName}-{Field}";

                    if (!dictFieldData.ContainsKey(key))
                        dictFieldData[key] = dr["Unit"].ToString();
                }

                DataTable YearlyTable = ds.Tables[2];

                foreach (DataRow dr in YearlyTable.Rows)
                {
                    string ComName = dr["Commodity_Name"].ToString();
                    string DataDate = Convert.ToDateTime(dr["Data_Date"]).ToShortDateString();
                    string DataYear = dr["Data_Year"].ToString();
                    string DataMonth = dr["Data_Month"].ToString();
                    string Field = dr["Field"].ToString();
                    string value = dr["DataValue"].ToString();
                    string key = $"{ComName}{DataDate}{DataYear}{DataMonth}";

                    if (dictRowData.ContainsKey(key))
                    {
                        dictRowData[key].Data.Add(Field, value);
                    }
                    else
                    {
                        dictRowData.Add(key, new RowData());
                        dictRowData[key].Commodity = ComName;
                        dictRowData[key].Data.Add("Commodity_Name", ComName);
                        dictRowData[key].Data.Add("Data_Date", DataDate);
                        dictRowData[key].Data.Add("Data_Year", DataYear);
                        dictRowData[key].Data.Add("Data_Month", DataMonth);
                        dictRowData[key].Data.Add(Field, value);
                    }
                }

                foreach (KeyValuePair<string, RowData> kv in dictRowData)
                {
                    RootData commData = dictRoot[kv.Value.Commodity];
                    RootData commDataYear = new RootData();
                    commDataYear.Name = $"{commData.Name} - Current Estimates";
                    commDataYear.TableName = $"WASDE_DOMESTIC_YEARLY_DATA";
                    commDataYear.Unit = commData.Unit;
                    commDataYear.Properties = commData.Properties;
                    List<string> lstRow = new List<string>();

                    if (dataSourceData.DictFormatedData.ContainsKey(commDataYear.Name))
                    {
                        foreach (KeyValuePair<string, string> rv in kv.Value.Data)
                        {
                            lstRow.Add(rv.Value);
                        }
                        dataSourceData.DictFormatedData[commDataYear.Name].Values.Add(lstRow);
                    }
                    else
                    {
                        FormatedData formatData = new FormatedData();

                        foreach (KeyValuePair<string, string> rv in kv.Value.Data)
                        {
                            bool bReadOnly = false;
                            if (rv.Key == "Commodity_Name" || rv.Key == "Data_Date" ||
                                rv.Key == "Data_Year" || rv.Key == "Data_Month")
                            {
                                bReadOnly = true;
                            }
                            string key = $"{commData.Name}-{rv.Key}";
                            formatData.Headers.Add(new HeaderFields()
                            {
                                Name = rv.Key,
                                DisplayName = rv.Key,
                                Unit = bReadOnly ? String.Empty : dictFieldData[key],
                                ReadOnly = bReadOnly
                            });
                            lstRow.Add(rv.Value);
                        }
                        formatData.Values.Add(lstRow);
                        formatData.CommodityData = commDataYear;
                        dataSourceData.DictFormatedData.Add(commDataYear.Name, formatData);
                    }
                }
                dictRowData.Clear();

                DataTable MonthlyData = ds.Tables[3];
                foreach (DataRow dr in MonthlyData.Rows)
                {
                    string ComName = dr["Commodity_Name"].ToString();
                    string DataDate = Convert.ToDateTime(dr["Data_Date"]).ToShortDateString();
                    string DataYear = dr["Data_Year"].ToString();
                    string DataMonth = dr["Data_Month"].ToString();
                    string Estonth = dr["Est_Month"].ToString();
                    string Field = dr["Field"].ToString();
                    string value = dr["DataValue"].ToString();
                    string key = $"{ComName}{DataDate}{DataYear}{DataMonth}{Estonth}";

                    if (dictRowData.ContainsKey(key))
                    {
                        dictRowData[key].Data.Add(Field, value);
                    }
                    else
                    {
                        dictRowData.Add(key, new RowData());
                        dictRowData[key].Commodity = ComName;
                        dictRowData[key].Data.Add("Commodity_Name", ComName);
                        dictRowData[key].Data.Add("Data_Date", DataDate);
                        dictRowData[key].Data.Add("Data_Year", DataYear);
                        dictRowData[key].Data.Add("Data_Month", DataMonth);
                        dictRowData[key].Data.Add("Est_Month", Estonth);
                        dictRowData[key].Data.Add(Field, value);
                    }
                }

                foreach (KeyValuePair<string, RowData> kv in dictRowData)
                {
                    RootData commData = dictRoot[kv.Value.Commodity];
                    RootData commDataYear = new RootData();
                    commDataYear.Name = $"{commData.Name}- Proj Estimates";
                    commDataYear.TableName = $"WASDE_DOMESTIC_MONTHLY_DATA";
                    commDataYear.Unit = commData.Unit;
                    commDataYear.Properties = commData.Properties;
                    List<string> lstRow = new List<string>();

                    if (dataSourceData.DictFormatedData.ContainsKey(commDataYear.Name))
                    {
                        foreach (KeyValuePair<string, string> rv in kv.Value.Data)
                        {
                            lstRow.Add(rv.Value);
                        }
                        dataSourceData.DictFormatedData[commDataYear.Name].Values.Add(lstRow);
                    }
                    else
                    {
                        FormatedData formatData = new FormatedData();

                        foreach (KeyValuePair<string, string> rv in kv.Value.Data)
                        {
                            bool bReadOnly = false;
                            if (rv.Key == "Commodity_Name" || rv.Key == "Data_Date" ||
                               rv.Key == "Est_Month" || rv.Key == "Data_Year" || rv.Key == "Data_Month")
                            {
                                bReadOnly = true;
                            }
                            string key = $"{commData.Name}-{rv.Key}";
                            formatData.Headers.Add(new HeaderFields()
                            {
                                Name = rv.Key,
                                DisplayName = rv.Key,
                                Unit = bReadOnly ? String.Empty : dictFieldData[key],
                                ReadOnly = bReadOnly
                            });
                            lstRow.Add(rv.Value);
                        }
                        formatData.Values.Add(lstRow);
                        formatData.CommodityData = commDataYear;
                        dataSourceData.DictFormatedData.Add(commDataYear.Name, formatData);
                    }
                }
            }
            return dataSourceData;
        }
        public Dictionary<string, WasdeCommodity> GetWasdeCommodities()
        {
            Dictionary<string, WasdeCommodity> lstWasdeComm = new Dictionary<string, WasdeCommodity>();
            using (IDbCommand dbCommand = dbHelper.CreateCommand("select * from WASDE_WORLD_COMMODITIES", CommandType.Text))
            {

                IDataReader dr = dbHelper.ExecuteReader();
                while (dr.Read())
                {
                    lstWasdeComm.Add(dr["Commodity_Category"].ToString().ToUpper().Trim(),
                    new WasdeCommodity()
                    {
                        CommodityName = dr["Commodity_Name"].ToString(),
                        Category = dr["Commodity_Category"].ToString(),
                        Unit = dr["Units"].ToString(),
                        bExists = true
                    });
                }
            }
            return lstWasdeComm;
        }
        public DSFormatedData GetCocoaFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null)
        {
            DSFormatedData dsFormatedData = new DSFormatedData();
            FormatedData forData = new FormatedData();
            forData.CommodityData = new RootData();
            forData.CommodityData.Name = "COCOA GRIND";
            forData.CommodityData.TableName = "COCOA_GRIND_DIALY_DATA";

            dsFormatedData.DictFormatedData.Add("COCOA GRIND", forData);
            try
            {
                using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_GET_COCOA_LAST_UPDATED", CommandType.StoredProcedure))
                {
                    dbHelper.AddParameter(dbCommand, "@INDEX", Index);
                    if (From.HasValue)
                        dbHelper.AddParameter(dbCommand, "@Start", From);
                    if (To.HasValue)
                        dbHelper.AddParameter(dbCommand, "@End", To);

                    DataSet ds = dbHelper.ExecuteDataSet();

                    DataTable Data = ds.Tables[0];
                    dbHelper.CloseConnection();

                    foreach (DataColumn dc in Data.Columns)
                    {
                        forData.Headers.Add(new HeaderFields()
                        {
                            Name = dc.ColumnName,
                            DisplayName = dc.ColumnName,
                            ReadOnly = (dc.ColumnName == "Cocoa_Grind") ? false : true
                        });
                    }

                    foreach (DataRow dr in Data.Rows)
                    {
                        List<string> row = new List<string>();
                        foreach (DataColumn dc in Data.Columns)
                        {
                            row.Add(dr[dc.ColumnName].ToString());
                        }
                        forData.Values.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return dsFormatedData;
        }

        public DSFormatedData GetCOTFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null)
        {
            DSFormatedData dsFormatedData = new DSFormatedData();
            FormatedData forData = new FormatedData();
            forData.CommodityData = new RootData();
            forData.CommodityData.Name = "COT";
            forData.CommodityData.TableName = "COT_DIALY_DATA";

            dsFormatedData.DictFormatedData.Add("COT", forData);
            try
            {
                using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_GET_COT_LAST_UPDATED", CommandType.StoredProcedure))
                {
                    dbHelper.AddParameter(dbCommand, "@INDEX", Index);
                    if (From.HasValue)
                        dbHelper.AddParameter(dbCommand, "@From", From);
                    if (To.HasValue)
                        dbHelper.AddParameter(dbCommand, "@To", To);

                    DataSet ds = dbHelper.ExecuteDataSet();

                    DataTable Data = ds.Tables[0];
                    dbHelper.CloseConnection();

                    foreach (DataColumn dc in Data.Columns)
                    {
                        forData.Headers.Add(new HeaderFields()
                        {
                            Name = dc.ColumnName,
                            DisplayName = dc.ColumnName,
                            ReadOnly = (dc.ColumnName == "COT") ? false : true
                        });
                    }

                    foreach (DataRow dr in Data.Rows)
                    {
                        List<string> row = new List<string>();
                        foreach (DataColumn dc in Data.Columns)
                        {
                            row.Add(dr[dc.ColumnName].ToString());
                        }
                        forData.Values.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return dsFormatedData;
        }
        public Dictionary<string, WasdeDomesticCommodity> GetWasdeDomesticCommodities()
        {
            Dictionary<string, WasdeDomesticCommodity> lstWasdeComm = new Dictionary<string, WasdeDomesticCommodity>();
            using (IDbCommand dbCommand = dbHelper.CreateCommand("select * from WASDE_DOMESTIC_COMMODITIES", CommandType.Text))
            {

                IDataReader dr = dbHelper.ExecuteReader();
                while (dr.Read())
                {
                    lstWasdeComm.Add(dr["Commodity_Category"].ToString().ToUpper().Trim(),
                    new WasdeDomesticCommodity()
                    {
                        CommodityName = dr["Commodity_Name"].ToString(),
                        Category = dr["Commodity_Category"].ToString(),
                        bExists = true
                    });
                }
            }
            return lstWasdeComm;
        }

        public Dictionary<string, WasdeDomesticField> GetWasdeDomesticFields()
        {
            Dictionary<string, WasdeDomesticField> lstWasdeComm = new Dictionary<string, WasdeDomesticField>();
            using (IDbCommand dbCommand = dbHelper.CreateCommand("select * from WASDE_DOMESTIC_FIELDS", CommandType.Text))
            {
                IDataReader dr = dbHelper.ExecuteReader();
                while (dr.Read())
                {
                    string Field = dr["Field"].ToString().Trim();
                    string Commodity = dr["Commodity_Name"].ToString().Trim();

                    lstWasdeComm.Add($"{Field.ToUpper()}--{Commodity.ToUpper()}",
                    new WasdeDomesticField()
                    {
                        FieldName = dr["Field"].ToString(),
                        Commodity = dr["Commodity_Name"].ToString(),
                        Unit = dr["Unit"].ToString(),
                        bExists = true

                    });
                }
            }
            return lstWasdeComm;
        }

        public int ProcessQuery(string Query)
        {
            int result = 0;

            using (IDbCommand dbCommand = dbHelper.CreateCommand(Query, CommandType.Text))
            {
                result = dbHelper.ExecuteNonQuery();
                dbHelper.CloseConnection();
            }
            return result;
        }

        public Dictionary<string, FOField> GetFOFields()
        {
            Dictionary<string, FOField> DictFOField = new Dictionary<string, FOField>();
            using (IDbCommand dbCommand = dbHelper.CreateCommand("select * from FATSANDOIL_FIELDS", CommandType.Text))
            {
                IDataReader dr = dbHelper.ExecuteReader();
                while (dr.Read())
                {
                    string Field = dr["Field"].ToString().Trim();
                    string Category = dr["Category"].ToString().Trim();

                    DictFOField.Add($"{Field.ToUpper()}--{Category.ToUpper()}",
                    new FOField()
                    {
                        Field = Field,
                        DisplayName = dr["DisplayName"].ToString(),
                        Category = Category,
                        Unit = dr["Unit"].ToString(),
                        bExists = true
                    });
                }
            }
            return DictFOField;
        }

        public DSFormatedData GetBHFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null)
        {
            DSFormatedData dataSourceData = new DSFormatedData();
            Dictionary<string, HeaderFields> dictHeader = new Dictionary<string, HeaderFields>();
            FormatedData formatedData = new FormatedData();

            using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_GET_BH_DATA", CommandType.StoredProcedure))
            {
                dbHelper.AddParameter(dbCommand, "@INDEX", Index);
                if (From.HasValue)
                    dbHelper.AddParameter(dbCommand, "@From", From);
                if (To.HasValue)
                    dbHelper.AddParameter(dbCommand, "@To", To);

                DataSet ds = dbHelper.ExecuteDataSet();
                dbHelper.CloseConnection();

                DataTable Fields = ds.Tables[0];

                foreach (DataRow dr in Fields.Rows)
                {
                    string Name = dr["Name"].ToString();
                    string DisplayName = dr["DisplayName"].ToString();
                    string Unit = dr["Unit"].ToString();
                    dictHeader[Name] = new HeaderFields()
                    {
                        Name = Name,
                        DisplayName = DisplayName,
                        Unit = Unit,
                        ReadOnly = false
                    };
                }

                DataTable BHData = ds.Tables[1];

                formatedData.CommodityData = new RootData()
                {
                    Name = "BROILER AND HATCHERY",
                    TableName = "BROHAT_DIALY_DATA"
                };
                foreach (DataColumn dc in BHData.Columns)
                {
                    if (dictHeader.ContainsKey(dc.ColumnName))
                    {
                        formatedData.Headers.Add(dictHeader[dc.ColumnName]);
                    }
                    else
                    {
                        formatedData.Headers.Add(new HeaderFields()
                        {
                            Name = dc.ColumnName,
                            DisplayName = dc.ColumnName,
                            ReadOnly = true
                        });
                    }
                }
                foreach (DataRow dr in BHData.Rows)
                {
                    List<string> row = new List<string>();
                    foreach (DataColumn dc in BHData.Columns)
                    {
                        row.Add(dr[dc.ColumnName].ToString());
                    }
                    formatedData.Values.Add(row);
                }
            }
            dataSourceData.DictFormatedData.Add("BROILER AND HATCHERY", formatedData);
            return dataSourceData;
        }

        public DSFormatedData GetCFFormatedData(int Index = 0, DateTime? From = null, DateTime? To = null)
        {
            DSFormatedData dataSourceData = new DSFormatedData();
            Dictionary<string, HeaderFields> dictHeader = new Dictionary<string, HeaderFields>();
            FormatedData formatedData = new FormatedData();

            using (IDbCommand dbCommand = dbHelper.CreateCommand("McF_GET_CF_DATA", CommandType.StoredProcedure))
            {
                dbHelper.AddParameter(dbCommand, "@INDEX", Index);
                if (From.HasValue)
                    dbHelper.AddParameter(dbCommand, "@From", From);
                if (To.HasValue)
                    dbHelper.AddParameter(dbCommand, "@To", To);

                DataSet ds = dbHelper.ExecuteDataSet();
                dbHelper.CloseConnection();

                DataTable Fields = ds.Tables[0];

                foreach (DataRow dr in Fields.Rows)
                {
                    string Name = dr["Name"].ToString();
                    string DisplayName = dr["DisplayName"].ToString();
                    string Unit = dr["Unit"].ToString();
                    dictHeader[Name] = new HeaderFields()
                    {
                        Name = Name,
                        DisplayName = DisplayName,
                        Unit = Unit,
                        ReadOnly = false
                    };
                }

                DataTable BHData = ds.Tables[1];

                formatedData.CommodityData = new RootData()
                {
                    Name = "CATTLE ON FEED",
                    TableName = "CATFEED_DIALY_DATA"
                };
                foreach (DataColumn dc in BHData.Columns)
                {
                    if (dictHeader.ContainsKey(dc.ColumnName))
                    {
                        formatedData.Headers.Add(dictHeader[dc.ColumnName]);
                    }
                    else
                    {
                        formatedData.Headers.Add(new HeaderFields()
                        {
                            Name = dc.ColumnName,
                            DisplayName = dc.ColumnName,
                            ReadOnly = true
                        });
                    }
                }
                foreach (DataRow dr in BHData.Rows)
                {
                    List<string> row = new List<string>();
                    foreach (DataColumn dc in BHData.Columns)
                    {
                        row.Add(dr[dc.ColumnName].ToString());
                    }
                    formatedData.Values.Add(row);
                }
            }
            dataSourceData.DictFormatedData.Add("CATTLE ON FEED", formatedData);
            return dataSourceData;
        }


        private Dictionary<string, List<DSColumnInfo>> GetColumnDetails(string ColumnFilters, string TableFilter)
        {
            Dictionary<string, List<DSColumnInfo>> dictColums = new Dictionary<string, List<DSColumnInfo>>();
            string table = String.Empty;
            string[] tables = TableFilter.Split(',');
            for (int i = 0; i < tables.Length; i++)
            {
                table += $"'{TableFilter}',";
            }
            table = table.Substring(0, table.Length - 1);

            string query = $"SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME IN ({table}) AND COLUMN_NAME IN ({ColumnFilters})";
            using (IDbCommand dbCommand = dbHelper.CreateCommand(query, CommandType.Text))
            {
                IDataReader dr = dbCommand.ExecuteReader();
                while (dr.Read())
                {
                    string columnName = dr["COLUMN_NAME"].ToString().ToUpper();
                    if (!dictColums.ContainsKey(columnName))
                        dictColums[columnName] = new List<DSColumnInfo>();
                    dictColums[columnName].Add(new DSColumnInfo()
                    {
                        Table = dr["TABLE_NAME"].ToString(),
                        Column = columnName,
                        DataType = dr["DATA_TYPE"].ToString()
                    });
                }
            }
            return dictColums;
        }

        private string GetUpdateString(Dictionary<string, List<DSColumnInfo>> dictColumns, string Column, string Value)
        {
            List<DSColumnInfo> lstCo = dictColumns[Column];

            switch (lstCo[0].DataType.ToUpper())
            {
                case "BIGINT":
                case "INT":
                case "BIT":
                case "DECIMAL":
                case "DOUBLE":
                    return Value;
                default:
                    return $"'{Value}'";
            }
        }

        private string FormatUpdateQuery(Dictionary<string, string> Keys,
            Dictionary<string, string> Values, Dictionary<string, List<DSColumnInfo>> dictColumns)
        {
            string WhereQuery = String.Empty;
            foreach (KeyValuePair<string, string> kv in Keys)
            {
                WhereQuery += $"{kv.Key}={GetUpdateString(dictColumns, kv.Key, kv.Value)} AND ";
            }
            WhereQuery = WhereQuery.Substring(0, WhereQuery.Length - 4);
            string ValueQuery = String.Empty;
            foreach (KeyValuePair<string, string> kv in Values)
            {
                ValueQuery += $"{kv.Key}={GetUpdateString(dictColumns, kv.Key, kv.Value)},";
            }
            ValueQuery = ValueQuery.Substring(0, ValueQuery.Length - 1);

            return $"UPDATE {dictColumns[Values.ElementAt(0).Key][0].Table} SET {ValueQuery} WHERE {WhereQuery}";
        }

        public bool SaveData(DSUpdatedData dsUpdatedData)
        {
            string columns = String.Empty;
            Dictionary<string, string> Keys = new Dictionary<string, string>();
            Dictionary<string, string> Values = new Dictionary<string, string>();
            foreach (UpdatedValues updateValues in dsUpdatedData.KeyData)
            {
                updateValues.Header = updateValues.Header.ToUpper();
                columns += $"'{updateValues.Header}',";
                Keys.Add(updateValues.Header, updateValues.Value);
            }
            foreach (UpdatedValues updateValues in dsUpdatedData.ValueData)
            {
                updateValues.Header = updateValues.Header.ToUpper();
                columns += $"'{updateValues.Header}',";
                Values.Add(updateValues.Header, updateValues.Value);
            }

            columns = columns.Substring(0, columns.Length - 1);

            Dictionary<string, List<DSColumnInfo>> dictColumns = GetColumnDetails(columns, dsUpdatedData.Table);
            string query = String.Empty;

            switch (dsUpdatedData.DataSource.ToUpper())
            {
                case "DTN":
                case "ETHANOL":
                case "USWeekly":
                case "COT":
                case "CATTLEONFEED":
                case "CHICKENANDEGGS":
                case "BROILERANDHATCHERY":
                case "HOGSANDPIGS":
                case "COCOA":
                case "FATSANDOILS":
                case "WASDEWORLD":
                case "WASDEDOMESTIC":
                    query = FormatUpdateQuery(Keys, Values, dictColumns);
                    break;
                case "SUGAR":

                    break;
                case "CROPPROGRESS":
                    break;
            }
            using (IDbCommand dbCommand = dbHelper.CreateCommand(query, CommandType.Text))
            {
                dbCommand.ExecuteNonQuery();
            }
            return true;
        }

        public DataSet ExecuteDataSetFromSP(string storedProc)
        {
            DataSet data = null;
            using (IDbCommand dbCommand = dbHelper.CreateCommand(storedProc, CommandType.StoredProcedure))
            {
                data = dbHelper.ExecuteDataSet();
                dbHelper.CloseConnection();
            }
            return data;
        }
        private DataTable CreateTable(List<string> symbols)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SYMBOL", typeof(string));
            foreach (string sym in symbols)
            {
                dt.Rows.Add(sym);
            }
            return dt;
        }
        public DataSet GetTableFormatedData(String SP, List<string> Symbols, int index, DateTime? From, DateTime? To)
        {
            DataSet ds = null;
            try
            {
                using (IDbCommand dbCommand = dbHelper.CreateCommand(SP, CommandType.StoredProcedure))
                {
                    DataTable dt = CreateTable(Symbols);
                    dbHelper.AddParameter(dbCommand, "@SymbolTable", dt);
                    dbHelper.AddParameter(dbCommand, "@INDEX", index);
                    dbHelper.AddParameter(dbCommand, "@From", From);
                    dbHelper.AddParameter(dbCommand, "@To", To);
                    ds = dbHelper.ExecuteDataSet();
                    dbHelper.CloseConnection();
                }
            }
            catch (Exception ex)
            {

            }
            return ds;
        }

        public DataSet ProcessDataQuery(string Query)
        {
            DataSet ds = null;
            try
            {
                using (IDbCommand dbCommand = dbHelper.CreateCommand(Query, CommandType.Text))
                {
                    ds = dbHelper.ExecuteDataSet();
                    dbHelper.CloseConnection();
                }
            }
            catch (Exception ex)
            {

            }
            return ds;
        }

        public DataSet DeleteExistingRecords(string SP, string Reportdate)
        {
            DataSet ds = null;
            try
            {
                using (IDbCommand dbCommand = dbHelper.CreateCommand(SP, CommandType.StoredProcedure))
                {
                    dbHelper.AddParameter(dbCommand, "@@ReportDate", Reportdate);
                    ds = dbHelper.ExecuteDataSet();
                    dbHelper.CloseConnection();
                }
            }
            catch (Exception ex)
            {

            }
            return ds;
        }
        public DataSet GetExcelFormatedData(ExcelQuery excelQuery)
        {
            DataSet ds = null;
            using (IDbCommand dbCommand = dbHelper.CreateCommand(excelQuery.StoredProc, CommandType.StoredProcedure))
            {
                if (excelQuery.Symbols != null && excelQuery.Symbols.Count > 0)
                    dbHelper.AddParameter(dbCommand, "@SymbolTable", CreateTable(excelQuery.Symbols));
                dbHelper.AddParameter(dbCommand, "@Index", excelQuery.DateIndex);
                if (excelQuery.StartDate.HasValue)
                    dbHelper.AddParameter(dbCommand, "@From", excelQuery.StartDate);
                if (excelQuery.EndDate.HasValue)
                    dbHelper.AddParameter(dbCommand, "@To", excelQuery.EndDate);
                if (!String.IsNullOrEmpty(excelQuery.PeriodType) && !String.IsNullOrEmpty(excelQuery.PeriodSummary))
                {
                    dbHelper.AddParameter(dbCommand, "@RollupValue", excelQuery.PeriodType);
                    dbHelper.AddParameter(dbCommand, "@RollupFequency", excelQuery.PeriodSummary);
                }
                if (excelQuery.FiscalMonth > 0)
                    dbHelper.AddParameter(dbCommand, "@FiscalMonth", excelQuery.FiscalMonth);
                ds = dbHelper.ExecuteDataSet();
                dbHelper.CloseConnection();
            }
            return ds;
        }

        public void BulkInsertDataTable(string tableName, DataTable table)
        {
            using (IDbConnection connection = dbHelper.OpenConnection())
            {
                SqlBulkCopy bulkCopy =
                    new SqlBulkCopy
                    (
                    (SqlConnection)connection
                    );

                bulkCopy.DestinationTableName = tableName;
                //   connection.Open();

                bulkCopy.WriteToServer(table);
                connection.Close();
            }
        }

        public DSFormatedData GetCompleteFormatData(string DataSource, string RootSource, int Index = 0, DateTime? From = null, DateTime? To = null)
        {
            DSFormatedData dataSourceData = new DSFormatedData();
            FormatedData formatedData = new FormatedData();

            ExcelQuery exclQuery = new ExcelQuery();
            exclQuery.DateIndex = Index;
            if (From.HasValue)
                exclQuery.StartDate = From;
            if (To.HasValue)
                exclQuery.EndDate = To;

            switch (DataSource)
            {
                case "CocoaSD":
                    formatedData.CommodityData = new RootData()
                    {
                        Name = "Cocoa Supply and Demand",
                        TableName = "COCOA_GRIND_ANNUAL_DATA"
                    };
                    exclQuery.StoredProc = "McF_GET_COCOA_ANNUAL_GRIND_EXCEL";
                    break;
                case "RegionalGrind":
                    formatedData.CommodityData = new RootData()
                    {
                        Name = "Regional Grind",
                        TableName = "COCOA_GRIND_DATA"
                    };
                    exclQuery.StoredProc = "McF_GET_COCOA_DATA_EXCEL";
                    break;
                case "BroilerAndHatchery":
                    formatedData.CommodityData = new RootData()
                    {
                        Name = "Broiler and Hatchery",
                        TableName = "BROHAT_DIALY_DATA"
                    };
                    exclQuery.StoredProc = "McF_GET_BH_DATA_EXCEL";
                    break;
                case "CattleOnFeed":
                    formatedData.CommodityData = new RootData()
                    {
                        Name = "Cattle on Feed",
                        TableName = "CATFEED_DIALY_DATA"
                    };
                    exclQuery.StoredProc = "McF_GET_CF_DATA_EXCEL";
                    break;
                case "ChickenAndEggs":
                    formatedData.CommodityData = new RootData()
                    {
                        Name = "Chicken and Eggs",
                        TableName = "CHICKENEGGS_DIALY_DATA"
                    };
                    exclQuery.StoredProc = "McF_GET_CE_DATA_EXCEL";
                    break;
                case "HogsAndPigs":
                    formatedData.CommodityData = new RootData()
                    {
                        Name = "Hogs and Pigs",
                        TableName = "HOGSPIGS_DIALY_DATA"
                    };
                    exclQuery.StoredProc = "McF_GET_HP_DATA_EXCEL";
                    break;
                case "Ethanol":
                    formatedData.CommodityData = new RootData()
                    {
                        Name = "Ethanol",
                        TableName = "ETHANOL_DIALY_DATA"
                    };
                    exclQuery.StoredProc = "McF_GET_ETHANOL_DATA_EXCEL";
                    break;
                case "TraderPositions":
                    formatedData.CommodityData = new RootData()
                    {
                        Name = "Trader Positions",
                        TableName = "COT_DIALY_DATA"
                    };
                    exclQuery.StoredProc = "McF_GET_COT_DATA_EXCEL";
                    break;
                case "USWeekly":
                    formatedData.CommodityData = new RootData()
                    {
                        Name = "USWeekly",
                        TableName = "USWEEKLY_DIALY_DATA"
                    };
                    exclQuery.StoredProc = "McF_GET_USWEEKLY_DATA_EXCEL";
                    break;
                case "ContinuousContracts":
                    formatedData.CommodityData = new RootData()
                    {
                        Name = "Continuous Contracts",
                        TableName = "DTN_DIALY_DATA"
                    };
                    exclQuery.StoredProc = "McF_GET_DTN_DATA_EXCEL";
                    break;
                case "MonthlyContracts":
                    formatedData.CommodityData = new RootData()
                    {
                        Name = "Monthly Contracts",
                        TableName = "DTN_DIALY_DATA"
                    };
                    exclQuery.StoredProc = "McF_GET_DTN_DATA_EXCEL";
                    break;
                case "PhysicalCommodities":
                    formatedData.CommodityData = new RootData()
                    {
                        Name = "Physical Commodities",
                        TableName = "PHYSICAL_COMMODITIES_DATA"
                    };
                    exclQuery.StoredProc = "McF_GET_PC_DATA_EXCEL";
                    break;
                case "BuyerData":
                    formatedData.CommodityData = new RootData()
                    {
                        Name = "Buyer Data",
                        TableName = "SUGAR_BUYER_DATA"
                    };
                    exclQuery.StoredProc = "McF_SUGAR_BUYER_DATA_EXCEL";
                    break;
                case "ImportData":
                    formatedData.CommodityData = new RootData()
                    {
                        Name = "Import Data",
                        TableName = "Sugar_ImportData"
                    };
                    exclQuery.StoredProc = "McF_SUGAR_IMPORT_DATA_EXCEL";
                    break;
                case "RegionData":
                    formatedData.CommodityData = new RootData()
                    {
                        Name = "Region Data",
                        TableName = "Sugar_RegionDelData"
                    };
                    exclQuery.StoredProc = "McF_SUGAR_REGION_DATA_EXCEL";
                    break;
                case "SupplyDemandData":
                    formatedData.CommodityData = new RootData()
                    {
                        Name = "Supply Demand Data",
                        TableName = "SUGAR_SUPPLY_DEMAND_DATA"
                    };
                    exclQuery.StoredProc = "McF_SUGAR_SUPPLY_DEMAND_DATA_EXCEL";
                    break;
                case "CANEBEETShare":
                    formatedData.CommodityData = new RootData()
                    {
                        Name = "CANE BEET Share",
                        TableName = "Sugar_USDACaneBeetShare"
                    };
                    exclQuery.StoredProc = "McF_GET_Sugar_USDACaneBeetShare_EXCEL";
                    break;
                case "HFCSImportsAndExports":
                    formatedData.CommodityData = new RootData()
                    {
                        Name = "HFCS Imports and Exports",
                        TableName = "Sweetner_USDAHFCSImportsExports"
                    };
                    exclQuery.StoredProc = "McF_GET_Sweetner_USDAHFCSImportsExports_EXCEL";
                    break;
                case "HFCSSupplyAndUse":
                    formatedData.CommodityData = new RootData()
                    {
                        Name = "HFCS Supply and Use",
                        TableName = "Sweetner_USDAHFCSSupplyUse"
                    };
                    exclQuery.StoredProc = "McF_GET_Sweetner_USDAHFCSSupplyUse_EXCEL";
                    break;
                case "SugarProductionArea":
                    formatedData.CommodityData = new RootData()
                    {
                        Name = "Sugar Production Area",
                        TableName = "Sugar_USDASugarProductionArea"
                    };
                    exclQuery.StoredProc = "McF_GET_Sugar_USDASugarProductionArea_EXCEL";
                    break;
                case "SugarProductionbyState":
                    formatedData.CommodityData = new RootData()
                    {
                        Name = "Sugar Production by State",
                        TableName = "Sugar_USDASugarProductionByState"
                    };
                    exclQuery.StoredProc = "McF_GET_Sugar_USDASugarProductionByState_EXCEL";
                    break;
                case "USDAPrices":
                    formatedData.CommodityData = new RootData()
                    {
                        Name = "USDA Prices",
                        TableName = "Sugar_USDAPrices"
                    };
                    exclQuery.StoredProc = "McF_GET_Sugar_USDAPrices_EXCEL";
                    break;
                case "USDAProduction":
                    formatedData.CommodityData = new RootData()
                    {
                        Name = "USDA Production",
                        TableName = "Sugar_USDAProduction"
                    };
                    exclQuery.StoredProc = "McF_GET_Sugar_USDAProduction_EXCEL";
                    break;
                case "WorldSweetenerData":
                    formatedData.CommodityData = new RootData()
                    {
                        Name = "World Sweetener Data",
                        TableName = "WORLD_SWEETENER_DATA"
                    };
                    exclQuery.StoredProc = "McF_GET_WORLD_SWEETENER_DATA_EXCEL";
                    break;
            }

            DataSet DBData = GetExcelFormatedData(exclQuery);
            Dictionary<string, HeaderFields> dictHeader = new Dictionary<string, HeaderFields>();

            if (DBData != null && DBData.Tables.Count == 2)
            {
                DataTable FieldInfo = DBData.Tables[0];
                DataTable Data = DBData.Tables[1];

                foreach (DataRow dr in FieldInfo.Rows)
                {
                    int PrimaryKey = Convert.ToInt16(dr["PrimaryKey"]);
                    string Name = dr["ColumnName"].ToString();
                    string DataType = dr["Datatype"].ToString();
                    string Table = dr["TableName"].ToString();
                    string DisplayName = dr["DisplayName"] == DBNull.Value ? Name : dr["DisplayName"].ToString();

                    if (PrimaryKey == 1)
                    {
                        dictHeader[Name] = new HeaderFields()
                        {
                            Name = Name,
                            DisplayName = DisplayName,
                            Unit = String.Empty,
                            ReadOnly = true
                        };
                    }
                    else
                    {
                        string Unit = dr["Unit"] == DBNull.Value ? String.Empty : dr["Unit"].ToString();

                        dictHeader[Name] = new HeaderFields()
                        {
                            Name = Name,
                            DisplayName = DisplayName,
                            Unit = Unit,
                            ReadOnly = false
                        };
                    }
                }

                foreach (DataRow dr in Data.Rows)
                {
                    List<string> row = new List<string>();
                    foreach (DataColumn dc in Data.Columns)
                    {
                        row.Add(dr[dc.ColumnName].ToString());
                    }
                    formatedData.Values.Add(row);
                }
            }
            dataSourceData.DictFormatedData.Add(formatedData.CommodityData.Name, formatedData);
            return dataSourceData;
        }
        public bool SaveDataSource(DataSource datasource)
        {
            DataSet ds = new DataSet();
            DataTable table = ds.Tables.Add(datasource.Name);

            Dictionary<int, DataCol> dc = new Dictionary<int, DataCol>();

            int columnIndex = 1;
            if (datasource.Headers.Count > 0 && datasource.Values.Count > 0)
            {
                foreach (Header kv in datasource.Headers)
                {
                    dc.Add(columnIndex, new DataCol(kv.Name) { Type = kv.Type, Unit = String.Empty });
                    columnIndex++;
                    switch (kv.Type.ToUpper())
                    {
                        case "INT":
                            table.Columns.Add(kv.Name, typeof(Int32));
                            break;
                        case "DECIMAL":
                            table.Columns.Add(kv.Name, typeof(double));
                            break;
                        case "STRING":
                            table.Columns.Add(kv.Name, typeof(String));
                            break;
                        case "DATE":
                            table.Columns.Add(kv.Name, typeof(DateTime));
                            break;
                        default:
                            table.Columns.Add(kv.Name, typeof(String));
                            break;
                    }
                }
            }
            foreach (List<string> lstData in datasource.Values)
            {
                object[] data = new object[datasource.Headers.Count];
                int index = 0;
                foreach (string val in lstData)
                {
                    data[index++] = val;
                }
                table.Rows.Add(data);
            }
            BulkInsertTable(datasource.Name, datasource.DisplayName, table, dc);
            return false;
        }

        public void BulkInsertTable(string table, string dSource, DataTable dTable, Dictionary<int, DataCol> dc)
        {
            string createQuery = SqlTableCreator.GetCreateFromDataTableSQL(table, dTable);
            string query = "INSERT INTO FREEFLOW_CONFIG SELECT ";
            foreach (KeyValuePair<int, DataCol> kv in dc)
            {
                string dataQuery = query + $"'{table}', '{kv.Value.Column}','{kv.Value.Column}','{kv.Value.Unit}'";
                ProcessQuery(dataQuery);
            }
            ProcessQuery($"INSERT INTO FREEFlowTables SELECT '{dSource}', '{table}', 0, '{DateTime.Now.ToShortDateString()}'");
            ProcessQuery(createQuery);
            BulkInsertDataTable(table, dTable);
        }


        public bool ResetPassword(string user, string passWord, string newPassWord)
        {
            if (ValidateUser(user, passWord))
            {
                ProcessQuery($"Update USERS set Password = '{newPassWord}' where UserName = '{user}'");
                return true;
            }
            return false;
        }
    }
}
