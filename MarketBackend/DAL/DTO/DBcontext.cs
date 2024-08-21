using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MarketBackend.DAL.DTO
{
    public class DBcontext : DbContext
    {
        private static DBcontext _instance = null;
        public static string DbPath;
        public static string DbPathRemote;
        public static string DbPathLocal;
        public static string DbPathTest;
        public static bool TestMode = false;
        public static bool RemoteMode = false;
        public static bool LocalMode = true;

        public virtual DbSet<MemberDTO> Members { get; set; }
        public virtual DbSet<MessageDTO> Messages { get; set; }
        public virtual DbSet<StoreDTO> Stores { get; set; }
        public virtual DbSet<RoleDTO> Roles { get; set; }

        public virtual DbSet<ShoppingCartDTO> ShoppingCarts { get; set; }
        public virtual DbSet<ShoppingCartHistoryDTO> ShoppingCartHistories { get; set; }
        public virtual DbSet<BasketDTO> Baskets { get; set; }
        public virtual DbSet<BasketItemDTO> BasketItems { get; set; }
        public virtual DbSet<ProductDTO> Products { get; set; }
        public virtual DbSet<PurchaseDTO> Purchases { get; set; }
        public virtual DbSet<EventDTO> Events { get; set; }

        //Policies
        public virtual DbSet<PolicyDTO> Policies { get; set; }
        public virtual DbSet<PolicySubjectDTO> PolicySubjects { get; set; }
        public virtual DbSet<PurchasePolicyDTO> PurchasePolicies { get; set; }
        public virtual DbSet<DiscountPolicyDTO> DiscountPolicies { get; set; }
        public virtual DbSet<DiscountCompositePolicyDTO> DiscountCompositePolicies { get; set; }

        //Rules
        public virtual DbSet<RuleDTO> Rules { get; set; }
        public virtual DbSet<RuleSubjectDTO> RuleSubjects { get; set; }
        public virtual DbSet<CompositeRuleDTO> CompositeRules { get; set; }
        public virtual DbSet<SimpleRuleDTO> SimplelRules { get; set; }
        public virtual DbSet<TotalPriceRuleDTO> TotalPriceRules { get; set; }
        public virtual DbSet<QuantityRuleDTO> QuantityRules { get; set; }



        public override void Dispose()
        {

            Events.ExecuteDelete();
            Rules.ExecuteDelete();
            RuleSubjects.ExecuteDelete();
            CompositeRules.ExecuteDelete();
            SimplelRules.ExecuteDelete();
            TotalPriceRules.ExecuteDelete();
            QuantityRules.ExecuteDelete();
            Policies.ExecuteDelete();
            PolicySubjects.ExecuteDelete();
            PurchasePolicies.ExecuteDelete();
            DiscountPolicies.ExecuteDelete();
            DiscountCompositePolicies.ExecuteDelete();
            Messages.ExecuteDelete();
            BasketItems.ExecuteDelete();
            Baskets.ExecuteDelete();
            Purchases.ExecuteDelete();
            ShoppingCartHistories.ExecuteDelete();
            ShoppingCarts.ExecuteDelete();
            Products.ExecuteDelete();
            Roles.ExecuteDelete();
            Stores.ExecuteDelete();
            Members.ExecuteDelete();

            SaveChanges();
            _instance = new DBcontext();
        }

        private static object _lock = new object();

        public static DBcontext GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new DBcontext();
                    }
                }
            }
            return _instance;
        }
        public DBcontext()
        {
            DbPathLocal = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MarketDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;Application Intent=ReadWrite;MultiSubnetFailover=False";
    
            DbPathRemote = "Server=tcp:market-db-server.database.windows.net,1433;Initial Catalog=MarketDB;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=\"Active Directory Default\";";
            
            DbPathTest = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MarketDBTest;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;Application Intent=ReadWrite;MultiSubnetFailover=False";
            
            if (RemoteMode)
                DbPath = DbPathRemote;
            else if (TestMode)
                DbPath = DbPathTest;
            else if (LocalMode)
                DbPath = DbPathLocal;
        }
        public static void SetLocalDB()
        {
            LocalMode = true;
            RemoteMode = false;
            TestMode = false;
            DbPath = DbPathLocal;
        }
        public static void SetRemoteDB()
        {
            LocalMode = false;
            RemoteMode = true;
            TestMode = false;
            DbPath = DbPathRemote;
        }

        public static void SetTestDB()
        {
            LocalMode = false;
            RemoteMode = false;
            TestMode = true;
            DbPath = DbPathTest;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                  optionsBuilder.UseSqlServer($"{DbPath}"); // Use DbPath to configure the database connection
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder){


            // RulesDTO

            modelBuilder.Entity<RuleDTO>()
                .HasDiscriminator<string>("Discriminator") // Specify the discriminator property name
                .HasValue<CompositeRuleDTO>("CompositeRule"); // Set the default discriminator value for the base class
            modelBuilder.Entity<RuleDTO>()
                .HasDiscriminator<string>("Discriminator") // Specify the discriminator property name
                .HasValue<SimpleRuleDTO>("SimpleRule"); // Set the default discriminator value for the base class
            modelBuilder.Entity<RuleDTO>()
                .HasDiscriminator<string>("Discriminator") // Specify the discriminator property name
                .HasValue<QuantityRuleDTO>("QuantityRule");
            modelBuilder.Entity<RuleDTO>()
                .HasDiscriminator<string>("Discriminator") // Specify the discriminator property name
                .HasValue<TotalPriceRuleDTO>("TotalPriceRule");
            modelBuilder.Entity<PolicyDTO>()
                .HasOne<PolicySubjectDTO>(p => p.PolicySubject)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<RuleDTO>()
                .HasOne<RuleSubjectDTO>(p => p.Subject)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);


            // PoliciesDTO

            modelBuilder.Entity<PolicyDTO>()
                .HasDiscriminator<string>("Discriminator") // Specify the discriminator property name
                .HasValue<DiscountPolicyDTO>("DiscountPolicy"); // Set the default discriminator value for the base class
            modelBuilder.Entity<PolicyDTO>()
                .HasDiscriminator<string>("Discriminator") // Specify the discriminator property name
                .HasValue<DiscountCompositePolicyDTO>("CompositeDiscountPolicy"); // Set the default discriminator value for the base class
            modelBuilder.Entity<PolicyDTO>()
                .HasDiscriminator<string>("Discriminator") // Specify the discriminator property name
                .HasValue<PurchasePolicyDTO>("PurchasePolicy");


            //policySubjectDTO

            modelBuilder.Entity<PolicySubjectDTO>()
                .HasOne<ProductDTO>(s => s.Product)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);


            //RuleSubjectDTO

            modelBuilder.Entity<RuleSubjectDTO>()
                .HasOne<ProductDTO>(s => s.Product)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);


            // MemberDTO

            modelBuilder.Entity<MemberDTO>()
                .HasMany<ShoppingCartHistoryDTO>(m => m.OrderHistory)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
            
            // modelBuilder.Entity<MemberDTO>()
            //     .HasMany<RoleDTO>(m => m.Roles)
            //     .WithOne()
            //     .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MemberDTO>()
                .HasMany<MessageDTO>(s => s.Alerts)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);


            // RoleDTO

            modelBuilder.Entity<RoleDTO>()
                .HasKey(r => new { r.storeId, r.userName });

            modelBuilder.Entity<RoleDTO>()
                .HasOne<StoreDTO>()
                .WithMany()
                .HasForeignKey(b => b.storeId)
                .OnDelete(DeleteBehavior.NoAction);

            // modelBuilder.Entity<RoleDTO>()
            //     .HasOne<MemberDTO>()
            //     .WithMany()
            //     .HasForeignKey(b => b.userName)
            //     .OnDelete(DeleteBehavior.NoAction);

            // modelBuilder.Entity<RoleDTO>()
            //     .HasOne<RoleTypeDTO>()
            //     .WithMany()
            //     .OnDelete(DeleteBehavior.NoAction);


            // modelBuilder.Entity<RoleDTO>()
            //     .HasOne<MemberDTO>(r => r.appointer)
            //     .WithMany()
            //     .OnDelete(DeleteBehavior.SetNull);

            // modelBuilder.Entity<RoleDTO>()
            //     .HasOne<MemberDTO>(s => s.appointer)
            //     .WithMany()
            //     .OnDelete(DeleteBehavior.NoAction);


            // ShoppingCartHistoryDTO

            modelBuilder.Entity<ShoppingCartHistoryDTO>()
                .HasMany<BasketDTO>(s => s._baskets)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ShoppingCartHistoryDTO>()
                .HasMany<ProductDTO>(s => s._products)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);


            // SHoppingCartDTO

            modelBuilder.Entity<ShoppingCartDTO>()
                .HasMany<BasketDTO>(s => s.Baskets)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);



            // BasketDTO

            modelBuilder.Entity<BasketDTO>()
                .HasOne<ShoppingCartDTO>()
                .WithMany(s => s.Baskets)
                .HasForeignKey(b => b.CartId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BasketDTO>()
                .HasOne<StoreDTO>()
                .WithMany()
                .HasForeignKey(b => b.StoreId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BasketDTO>()
                .HasMany<BasketItemDTO>(b => b.BasketItems)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);


            // BasketItemDTO

            modelBuilder.Entity<BasketItemDTO>()
                .HasOne<ProductDTO>(b => b.Product)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            
            // PurchaseDTO

            // modelBuilder.Entity<PurchaseDTO>()
            //     // .HasOne<BasketDTO>(p => p.Basket)
            //     .WithOne()
            //     .OnDelete(DeleteBehavior.SetNull);

            
            // EventDTO

            modelBuilder.Entity<EventDTO>()
                .HasOne<MemberDTO>(e => e.Listener)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<EventDTO>()
                .HasOne<StoreDTO>()
                .WithMany()
                .HasForeignKey(b => b.StoreId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}