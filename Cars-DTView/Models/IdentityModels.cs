using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Cars_DTView.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Car> Cars { get; set; }

        public async Task<List<Car>> GetCars(string year, string make, string model, string trim,
                                             string filter, bool? paging, int? page, int? perPage)
        {
            var yearParam = new SqlParameter("@year", year ?? "");
            var makeParam = new SqlParameter("@make", make ?? "");
            var modelParam = new SqlParameter("@model", model ?? "");
            var trimParam = new SqlParameter("@trim", trim ?? "");
            var filterParam = new SqlParameter("@filter", filter ?? "");

            var query = "GetCars @year,@make,@model,@trim,@filter";

            if (paging != null && paging.Value == true)
            {
                var pagingParam = new SqlParameter("@paging", paging);
                var pageParam = new SqlParameter("@page", page);
                var perPageParam = new SqlParameter("@perPage", perPage);
                query += ",@paging,@page,@perPage";
                try
                {
                    var result = await this.Database.SqlQuery<Car>(query, yearParam, makeParam, modelParam,
                                    trimParam, filterParam, pagingParam, pageParam, perPageParam).ToListAsync();
                    return result;
                }   
                catch (Exception e)
                {

                }
            }
            else
            {
                var result = await this.Database.SqlQuery<Car>(query, yearParam, makeParam, modelParam,
                                trimParam, filterParam).ToListAsync();
                return result;
            }
            return new List<Car>();
        }



        public async Task<List<string>> GetYears()
        {

            return await this.Database
                .SqlQuery<string>("GetYears").ToListAsync();
        }

        public async Task<List<string>> GetMakes(string year)
        {
            var yearParam = new SqlParameter("@year", year);
            return await this.Database
                .SqlQuery<string>("GetMakes @year", yearParam).ToListAsync();
        }

        public async Task<List<string>> GetModels(string year, string make)
        {
            var yearParam = new SqlParameter("@year", year);
            var makeParam = new SqlParameter("@make", make);
            return await this.Database
                .SqlQuery<string>("GetModels @year,@make", yearParam, makeParam).ToListAsync();
        }

        public async Task<List<string>> GetTrims(string year, string make, string model)
        {
            var yearParam = new SqlParameter("@year", year);
            var makeParam = new SqlParameter("@make", make);
            var modelParam = new SqlParameter("@model", model);
            return await this.Database
                .SqlQuery<string>("GetTrims @year,@make,@model", yearParam, makeParam, modelParam).ToListAsync();
        }
        public async Task<int> GetCarsCount(string year, string make, string model, string trim,
                                           string filter)
        {
            var query = @"SELECT COUNT(*) 
            FROM (SELECT DISTINCT *
			    FROM Cars 
				WHERE (@year is null OR @year = '' OR @year = model_year) AND 
					  (@make is null OR @make = '' OR @make = make) AND 
					  (@model is null OR @model = '' OR @model = model_name) AND 
					  (@trim is null OR @trim = '' OR @trim = model_trim) AND
					  (@filter is null OR 
							(
								model_year LIKE  '%' + @filter + '%' OR
								make LIKE  '%' + @filter + '%' OR
								model_name LIKE  '%' + @filter + '%' OR
								model_trim LIKE  '%' + @filter + '%' OR
								body_style LIKE  '%' + @filter + '%' OR
								engine_position LIKE  '%' + @filter + '%' OR
								engine_num_cyl LIKE  '%' + @filter + '%' OR
								engine_fuel LIKE  '%' + @filter  + '%' OR
								drive_type LIKE  '%' + @filter + '%' 
							)
						)
                ) AS T";
            var yearParam = new SqlParameter("@year", year);
            var makeParam = new SqlParameter("@make", make);
            var modelParam = new SqlParameter("@model", model);
            var trimParam = new SqlParameter("@trim", trim);
            var filterParam = new SqlParameter("@filter", filter ?? "");
            try
            {
                var result = await Database.SqlQuery<int>(query, yearParam, makeParam, modelParam,
                                trimParam, filterParam).FirstAsync();
                return result;
            }
            catch (Exception e) { }
            return 0;
        }
    }
}