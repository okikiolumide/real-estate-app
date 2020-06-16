using System;
namespace EstateApp.Data.Entities
{
    public abstract class BaseEntity
    {
        public int ID { get; set;}
        public bool IsDeleted { get; set;}
        public DateTime CreatedAt { get; set;}
        public DateTime ModifiedAt { get; set;}
        public DateTime DeletedAt { get; set;}
    }
}