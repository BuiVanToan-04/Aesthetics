using Aesthetics.DTO.NetCore.DataObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using XAct.Users;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Aesthetics.DataAccess.NetCore.DBContext
{
	public class DB_Context : Microsoft.EntityFrameworkCore.DbContext
	{
		public DB_Context(DbContextOptions options) : base(options) { }
		protected override void OnModelCreating(ModelBuilder builder)
		{
			//1.Chỉ định mối quan hệ 1-1 của Users & Carts
			builder.Entity<Users>()
				.HasOne(u => u.Cart)
				.WithOne(c => c.Users)
				.HasForeignKey<Carts>(c => c.UserID);

			//2.Chỉ định mối quan hệ 1-N của Permission(N) & Functions(1)
			builder.Entity<Permission>()
				.HasOne(f => f.Functions)              
				.WithMany(p => p.Permission)            
				.HasForeignKey(s => s.FunctionID);     

			//3.Chỉ định mối quan hệ 1-N của Users(1) & Permission(N)
			builder.Entity<Permission>()
				.HasOne(u => u.Users)                   
				.WithMany(p => p.Permissions)           
				.HasForeignKey(s => s.UserID);          

			//4.Chỉ định mối quan hệ của Carts & Products qua bảng trung gian CartProduct
			builder.Entity<CartProduct>()
				.HasKey(cp => cp.CartProductID);        
			builder.Entity<CartProduct>()
				.HasOne(c => c.Carts)                   
				.WithMany(cp => cp.CartProducts)        
				.HasForeignKey(s => s.CartID);          
			builder.Entity<CartProduct>()
				.HasOne(p => p.Products)                
				.WithMany(cp => cp.CartProducts)        
				.HasForeignKey(v => v.ProductID);       

			//5.Chỉ định mối quan hệ của 1-N của:
			//5.1.TypeProductsOfServices(1) & Products(N)
			builder.Entity<Products>()
				.HasOne(t => t.TypeProductsOfServices)  
				.WithMany(p => p.Products)             
				.HasForeignKey(s => s.ProductsOfServicesID);

			//5.2.TypeProductsOfServices(1) & Servicess(N)
			builder.Entity<Servicess>()                 
				.HasOne(t => t.TypeProductsOfServices)  
				.WithMany(s => s.Services)              
				.HasForeignKey(v => v.ProductsOfServicesID);

			//6.Chỉ định mối quan hệ N-N của Users & Clinic thông qua bảng trung gian Clinic_Staff
			builder.Entity<Clinic_Staff>()
				.HasKey(cs => cs.ClinicStaffID);
			builder.Entity<Clinic_Staff>()
				.HasOne(u => u.Users)
				.WithMany(us => us.Clinic_Staff)
				.HasForeignKey(s => s.UserID);
			builder.Entity<Clinic_Staff>()
				.HasOne(a => a.Clinic)
				.WithMany(w => w.Clinic_Staff)
				.HasForeignKey(v =>v.ClinicID);

			//7.Chỉ định mối quan hệ N - N của Booking & Servicess qua bảng trung gian Booking_Servicess
			builder.Entity<Booking_Servicess>()
				.HasKey(bs => bs.BookingServiceID);
			builder.Entity<Booking_Servicess>()
				.HasOne(b => b.Booking)
				.WithMany(s => s.Booking_Servicesses)
				.HasForeignKey(a => a.BookingID);
			builder.Entity<Booking_Servicess>()
				.HasOne(s => s.Servicess)
				.WithMany(b => b.Booking_Servicesses)
				.HasForeignKey(c => c.ServiceID);

			//8.Chỉ định mối quan hệ 1 - N của Booking_Servicess(1) - Booking_Assignment(N)
			builder.Entity<Booking_Assignment>()
				.HasOne(bs => bs.Booking_Servicess)
				.WithMany(ba => ba.Booking_Assignment)
				.HasForeignKey(a => a.BookingServiceID);
		}
		public virtual DbSet<Booking> Booking { get; set; }
		public virtual DbSet<Carts> Carts { get; set; }
		public virtual DbSet<CartProduct> CartProduct { get; set; }
		public virtual DbSet<Comments> Comments { get; set; }
		public virtual DbSet<Functions> Functions { get; set; }
		public virtual DbSet<Invoice> Invoice { get; set; }
		public virtual DbSet<InvoiceDetail> InvoiceDetail { get; set; }
		public virtual DbSet<Permission> Permission { get; set; }
		public virtual DbSet<Products> Products { get; set; }
		public virtual DbSet<Servicess> Servicess { get; set; }
		public virtual DbSet<Supplier> Supplier { get; set; }
		public virtual DbSet<TypeProductsOfServices> TypeProductsOfServices { get; set; }
		public virtual DbSet<Users> Users { get; set; }
		public virtual DbSet<UserSession> UserSession { get; set; }
		public virtual DbSet<Vouchers> Vouchers { get; set; }
		public virtual DbSet<Wallets> Wallets { get; set; }
		public virtual DbSet<Booking_Assignment> Booking_Assignment { get; set; }
		public virtual DbSet<Booking_Servicess> Booking_Servicess { get; set; }
		public virtual DbSet<Clinic> Clinic { get; set; }
		public virtual DbSet<Clinic_Staff> Clinic_Staff { get; set; }
	}
}
