using CsvHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RouteOptimizer;
using RouteOptimizer.Entity;
using RouteOptimizer.CF;
using RouteOptimizer.Entity;
namespace RouteOptimizer.BL
{
    public class SettingBL :CommonFunction
    { 
        static string DataSourceInstrumentList = Entity.StaticCache.DataSourceInstrumentList;
        static string DataSourceSystemList = Entity.StaticCache.DataSourceSystemList;
        static string DataSourceInstCable = Entity.StaticCache.DataSourceInstCable; 
        static string DataSourceCableList = Entity.StaticCache.DataSourceCableList;
        static string DataSourceCableDuctList = Entity.StaticCache.DataSourceCableDuctList;
        static string DataSourceSignalList = Entity.StaticCache.DataSourceSignal;//GetCableDuctTypeList
        static string DataSourceCableDuctTypeList = Entity.StaticCache.GetCableDuctTypeList;//GetCableDuctTypeList
        static string DataSourceMccPkgList = Entity.StaticCache.DataSourceMccPkgList;
        public List<MCCEntity> GetMCCPkgList()
        {
            List<MCCEntity> result = new List<MCCEntity>();
            using (TextReader fileReader = File.OpenText(DataSourceMccPkgList))
            {
                var csv = new CsvReader(fileReader);
                csv.Configuration.HasHeaderRecord = false;
                csv.Configuration.MissingFieldFound = null;
                csv.Read();
                result = csv.GetRecords<MCCEntity>().ToList();

            }
            return result;
        }
        public List<InstrumentListEntity> GetInstrumetnType()
        {
            List<InstrumentListEntity> result = new List<InstrumentListEntity>();
            using (TextReader fileReader = File.OpenText(DataSourceInstrumentList))
            {
                var csv = new CsvReader(fileReader);
                csv.Configuration.HasHeaderRecord = false;
                csv.Configuration.MissingFieldFound = null;
                csv.Read();
                result = csv.GetRecords<InstrumentListEntity>().ToList(); 
            }
            return result; 
        }
        public List<SignalListEntity> GetSignalType()
        {
            List<SignalListEntity> result = new List<SignalListEntity>();
            using (TextReader fileReader = File.OpenText(DataSourceSignalList))
            {
                var csv = new CsvReader(fileReader);
                csv.Configuration.HasHeaderRecord = false;
                csv.Configuration.MissingFieldFound = null;
                csv.Read();
                result = csv.GetRecords<SignalListEntity>().ToList();
            }
            return result;
        }
        public List<InstCableEntity> GetInsCable()  
        {
            List<InstCableEntity> result = new List<InstCableEntity>();
            using (TextReader fileReader = File.OpenText(DataSourceInstCable))
            {
                var csv = new CsvReader(fileReader);
                csv.Configuration.HasHeaderRecord = false  ;
                csv.Configuration.MissingFieldFound = null;
                csv.Read();
                result = csv.GetRecords<InstCableEntity>().ToList();
            }
            return result;
        }
        public List<CableDuctTypeListEntity> GetCableDuctTypeList()
        {
            List<CableDuctTypeListEntity> result = new List<CableDuctTypeListEntity>();
            using (TextReader fileReader = File.OpenText(DataSourceCableDuctTypeList))
            {
                var csv = new CsvReader(fileReader);
                csv.Configuration.HasHeaderRecord = false;
                csv.Configuration.MissingFieldFound = null;
                csv.Read();
                result = csv.GetRecords<CableDuctTypeListEntity>().ToList();
            }
            return result;
        }
        public List<CableListEntity> GetCableList()
        {
            List<CableListEntity> result = new List<CableListEntity>();
            using (TextReader fileReader = File.OpenText(DataSourceCableList))
            {
                var csv = new CsvReader(fileReader);
                csv.Configuration.HasHeaderRecord = false;
                csv.Configuration.MissingFieldFound = null;
                csv.Read();
                result = csv.GetRecords<CableListEntity>().ToList();
            }
            return result;
        }
        public List<CableDuctList> GetCableDuctList()
        {
            List<CableDuctList> result = new List<CableDuctList>();
            using (TextReader fileReader = File.OpenText(DataSourceCableDuctList))
            {
                var csv = new CsvReader(fileReader);
                csv.Configuration.HasHeaderRecord = false;
                csv.Configuration.MissingFieldFound = null;
                csv.Read();
                result = csv.GetRecords<CableDuctList>().ToList();
            }
            return result;
        }
        public List<SystemInfoEntity> GetSystemList()
        {
            ////MessageBox.Show(DataSourceSystemList);
            List<SystemInfoEntity> result = new List<SystemInfoEntity>();
            using (TextReader fileReader = File.OpenText(DataSourceSystemList))
            {
               
                var csv = new CsvReader(fileReader);
                csv.Configuration.HasHeaderRecord = false;
                csv.Configuration.MissingFieldFound = null;
                csv.Read();
                
                result = csv.GetRecords<SystemInfoEntity>().ToList();

            }
            return result;
        }
        public List<SystemInfoEntity> GetClassification(string Type1=null)
        {
            
            List<SystemInfoEntity> result = new List<SystemInfoEntity>();
            using (TextReader fileReader = File.OpenText(DataSourceSystemList))
            { 
                var csv = new CsvReader(fileReader);
                csv.Configuration.HasHeaderRecord = false;
                csv.Configuration.MissingFieldFound = null;
                csv.Read();

                result = csv.GetRecords<SystemInfoEntity>().ToList();

            }
            return result;
        }
    }
}
