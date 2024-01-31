using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AgileRap_Process2.Data;
using Microsoft.EntityFrameworkCore;

namespace AgileRap_Process2.Models
{
    public class ProviderLogMetadata
    {

    }

    [ModelMetadataType(typeof(ProviderLogMetadata))]

    public partial class ProviderLog
    {
        public void Insert(AgileRap_Process2Context dbContext)
        {
            this.CreateBy = GlobalVariable.GetUserID();
            this.UpdateBy = GlobalVariable.GetUserID();
            this.CreateDate = DateTime.Now;
            this.UpdateDate = DateTime.Now;
            this.IsDelete = false;
            dbContext.ProviderLog.Add(this);
        }

        public void Update(AgileRap_Process2Context dbContext)
        {
            this.UpdateBy = GlobalVariable.GetUserID();
            this.UpdateDate = DateTime.Now;
            var existingEntity = dbContext.ProviderLog.Find(this.ID);
            dbContext.Entry(existingEntity).CurrentValues.SetValues(this);
        }

        public void Delete(AgileRap_Process2Context dbContext)
        {
            var data = dbContext.ProviderLog.Find(this.ID);
            data.UpdateBy = GlobalVariable.GetUserID();
            data.UpdateDate = DateTime.Now;
            data.IsDelete = true;
            dbContext.Entry(data).State = EntityState.Modified;
        }
    }
}
