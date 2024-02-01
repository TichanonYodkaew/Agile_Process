using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AgileRap_Process2.Data;
using Microsoft.EntityFrameworkCore;


namespace AgileRap_Process2.Models
{
    public class UserMetadata
    {
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

    }

    [ModelMetadataType(typeof(UserMetadata))]
    public partial class User
    {
        
        [DataType(DataType.Password)]
        [NotMapped]
        public string? ConfirmPassword { get; set; }

        public void Insert(AgileRap_Process2Context dbContext)
        {
            this.Password = HashingHelper.HashPassword(this.Password);
            this.CreateDate = DateTime.Now;
            this.UpdateDate = DateTime.Now;
            this.IsDelete = false;
            dbContext.User.Add(this);
        }

        public void Update(AgileRap_Process2Context dbContext)
        {
            this.UpdateBy = GlobalVariable.GetUserID();
            this.UpdateDate = DateTime.Now;
            var existingEntity = dbContext.User.Find(this.ID);
            dbContext.Entry(existingEntity).CurrentValues.SetValues(this);
        }

        public void Delete(AgileRap_Process2Context dbContext)
        {
            var data = dbContext.User.Find(this.ID);
            data.UpdateBy = GlobalVariable.GetUserID();
            data.UpdateDate = DateTime.Now;
            data.IsDelete = true;
            dbContext.Entry(data).State = EntityState.Modified;
        }

        //public Work CheckDB (AgileRap_Process2Context dbContext)
        //{
        //    var data = dbContext.User.Where(m => m.Email == this.Email && m.Password == this.Password).FirstOrDefault();

        //    return data;
        //}
    }
    
}
