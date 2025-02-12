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
	}
}
