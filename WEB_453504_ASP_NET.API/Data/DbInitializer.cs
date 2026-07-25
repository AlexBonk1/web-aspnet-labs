using WEB_453504_ASP_NET.Domain.Entities;

namespace WEB_453504_ASP_NET.API.Data
{
    public class DbInitializer
    {
        public static async Task SeedData(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if (context == null)
            {
                throw new NullReferenceException("Db context is null");
            }
            
            // Добавляем категории
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Гитары", NormalizedName = "guitars" },
                new Category { Id = 2, Name = "Басс гитары", NormalizedName = "bass-guitars" },
                new Category { Id = 3, Name = "Скрипки", NormalizedName = "violins" },
                new Category { Id = 4, Name = "Акустические гитары", NormalizedName = "acoustic-guitars" },
                new Category { Id = 5, Name = "Барабаны", NormalizedName = "drums" }
            };

            // Проверяем, есть ли уже категории
            if (!context.Categories.Any())
            {
                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            // Если инструменты уже есть, выходим
            if (context.MusicalInstruments.Any())
            {
                return;
            }

            var g = (Func<string, Category?>)(n => context.Categories.FirstOrDefault(c => c.NormalizedName == n));

            var instruments = new List<MusicalInstrument>
            {
                // Гитары (5 инструментов)
                new MusicalInstrument
                {
                    Name = "Fender Stratocaster",
                    Description = "Классическая электрогитара с тремоло",
                    ImageUrl = "fender-stratocaster.jpg",
                    CategoryId = g("guitars")?.Id ?? 1,
                    Price = 1299.99m,
                    WeightKg = 3.5
                },
                new MusicalInstrument
                {
                    Name = "Gibson Les Paul",
                    Description = "Легендарная гитара с толстым звуком",
                    ImageUrl = "gibson-lespaul.jpg",
                    CategoryId = g("guitars")?.Id ?? 1,
                    Price = 2499.50m,
                    WeightKg = 4.2
                },
                new MusicalInstrument
                {
                    Name = "PRS Custom 24",
                    Description = "Премиум электрогитара с уникальным дизайном",
                    ImageUrl = "prs-custom24.jpg",
                    CategoryId = g("guitars")?.Id ?? 1,
                    Price = 3199.00m,
                    WeightKg = 3.8
                },
                new MusicalInstrument
                {
                    Name = "Ibanez RG550",
                    Description = "Скоростная гитара для металла",
                    ImageUrl = "ibanez-rg550.jpg",
                    CategoryId = g("guitars")?.Id ?? 1,
                    Price = 899.99m,
                    WeightKg = 3.2
                },
                new MusicalInstrument
                {
                    Name = "ESP LTD EC-256",
                    Description = "Универсальная гитара для различных стилей",
                    ImageUrl = "esp-ltd-ec256.jpg",
                    CategoryId = g("guitars")?.Id ?? 1,
                    Price = 749.00m,
                    WeightKg = 3.6
                },

                // Басс-гитары (5 инструментов)
                new MusicalInstrument
                {
                    Name = "Fender Precision Bass",
                    Description = "Классический четырёхструнный бас",
                    ImageUrl = "fender-precision-bass.jpg",
                    CategoryId = g("bass-guitars")?.Id ?? 2,
                    Price = 899.00m,
                    WeightKg = 4.0
                },
                new MusicalInstrument
                {
                    Name = "Ibanez SR500",
                    Description = "Современный бас с тонким грифом",
                    ImageUrl = "ibanez-sr500.jpg",
                    CategoryId = g("bass-guitars")?.Id ?? 2,
                    Price = 699.00m,
                    WeightKg = 3.8
                },
                new MusicalInstrument
                {
                    Name = "Yamaha BB434",
                    Description = "Качественный бас среднего ценового диапазона",
                    ImageUrl = "yamaha-bb434.jpg",
                    CategoryId = g("bass-guitars")?.Id ?? 2,
                    Price = 549.00m,
                    WeightKg = 3.9
                },
                new MusicalInstrument
                {
                    Name = "Warwick Rockbass Corvette",
                    Description = "Пятиструнный бас немецкого качества",
                    ImageUrl = "warwick-rockbass.jpg",
                    CategoryId = g("bass-guitars")?.Id ?? 2,
                    Price = 1299.00m,
                    WeightKg = 4.3
                },
                new MusicalInstrument
                {
                    Name = "Squier Jazz Bass",
                    Description = "Доступный бас для начинающих",
                    ImageUrl = "squier-jazz-bass.jpg",
                    CategoryId = g("bass-guitars")?.Id ?? 2,
                    Price = 399.00m,
                    WeightKg = 3.7
                },

                // Скрипки (5 инструментов)
                new MusicalInstrument
                {
                    Name = "Stentor Student II",
                    Description = "Учебная скрипка для начинающих",
                    ImageUrl = "stentor-student-ii.jpg",
                    CategoryId = g("violins")?.Id ?? 3,
                    Price = 199.99m,
                    WeightKg = 0.6
                },
                new MusicalInstrument
                {
                    Name = "Cremona SV-500",
                    Description = "Скрипка среднего уровня с хорошим звуком",
                    ImageUrl = "cremona-sv500.jpg",
                    CategoryId = g("violins")?.Id ?? 3,
                    Price = 749.00m,
                    WeightKg = 0.7
                },
                new MusicalInstrument
                {
                    Name = "Yamaha V5",
                    Description = "Профессиональная скрипка японского производства",
                    ImageUrl = "yamaha-v5.jpg",
                    CategoryId = g("violins")?.Id ?? 3,
                    Price = 1899.00m,
                    WeightKg = 0.8
                },
                new MusicalInstrument
                {
                    Name = "Hidersine Vivente",
                    Description = "Скрипка с отличным звучанием и деревом",
                    ImageUrl = "hidersine-vivente.jpg",
                    CategoryId = g("violins")?.Id ?? 3,
                    Price = 599.00m,
                    WeightKg = 0.65
                },
                new MusicalInstrument
                {
                    Name = "Karl Höfner Concertino",
                    Description = "Немецкая скрипка премиум класса",
                    ImageUrl = "hofner-concertino.jpg",
                    CategoryId = g("violins")?.Id ?? 3,
                    Price = 2499.00m,
                    WeightKg = 0.75
                },

                // Акустические гитары (5 инструментов)
                new MusicalInstrument
                {
                    Name = "Yamaha FG800",
                    Description = "Надёжная акустическая гитара для всех",
                    ImageUrl = "yamaha-fg800.jpg",
                    CategoryId = g("acoustic-guitars")?.Id ?? 4,
                    Price = 199.00m,
                    WeightKg = 2.1
                },
                new MusicalInstrument
                {
                    Name = "Takamine GD30",
                    Description = "Акустическая гитара среднего класса",
                    ImageUrl = "takamine-gd30.jpg",
                    CategoryId = g("acoustic-guitars")?.Id ?? 4,
                    Price = 349.00m,
                    WeightKg = 2.3
                },
                new MusicalInstrument
                {
                    Name = "Martin D-28",
                    Description = "Легендарная акустическая гитара премиум класса",
                    ImageUrl = "martin-d28.jpg",
                    CategoryId = g("acoustic-guitars")?.Id ?? 4,
                    Price = 3899.00m,
                    WeightKg = 2.5
                },
                new MusicalInstrument
                {
                    Name = "Taylor 114",
                    Description = "Компактная акустическая гитара для путешествий",
                    ImageUrl = "taylor-114.jpg",
                    CategoryId = g("acoustic-guitars")?.Id ?? 4,
                    Price = 299.00m,
                    WeightKg = 1.9
                },
                new MusicalInstrument
                {
                    Name = "Epiphone FT-100",
                    Description = "Фольк-гитара с красивым звучанием",
                    ImageUrl = "epiphone-ft100.jpg",
                    CategoryId = g("acoustic-guitars")?.Id ?? 4,
                    Price = 229.00m,
                    WeightKg = 2.2
                },

                // Барабаны (5 инструментов)
                new MusicalInstrument
                {
                    Name = "Pearl Export",
                    Description = "Ударная установка для репетиций и выступлений",
                    ImageUrl = "pearl-export.jpg",
                    CategoryId = g("drums")?.Id ?? 5,
                    Price = 599.00m,
                    WeightKg = 18.0
                },
                new MusicalInstrument
                {
                    Name = "Ludwig Accent",
                    Description = "Комплект барабанов для начинающих",
                    ImageUrl = "ludwig-accent.jpg",
                    CategoryId = g("drums")?.Id ?? 5,
                    Price = 449.00m,
                    WeightKg = 16.5
                },
                new MusicalInstrument
                {
                    Name = "Yamaha Stage Custom",
                    Description = "Профессиональные барабаны для студии и сцены",
                    ImageUrl = "yamaha-stage-custom.jpg",
                    CategoryId = g("drums")?.Id ?? 5,
                    Price = 1299.00m,
                    WeightKg = 22.0
                },
                new MusicalInstrument
                {
                    Name = "Sonor AQX",
                    Description = "Немецкие барабаны с отличным звуком",
                    ImageUrl = "sonor-aqx.jpg",
                    CategoryId = g("drums")?.Id ?? 5,
                    Price = 899.00m,
                    WeightKg = 20.0
                },
                new MusicalInstrument
                {
                    Name = "Mapex Tornado",
                    Description = "Компактный комплект барабанов для малых пространств",
                    ImageUrl = "mapex-tornado.jpg",
                    CategoryId = g("drums")?.Id ?? 5,
                    Price = 349.00m,
                    WeightKg = 14.5
                }
            };
            var adress = app.Configuration["ApplicationAdress"] ?? "https://localhost:7002";
            foreach(var instr in instruments)
            {
                instr.ImageUrl = Path.Combine(adress, "wwwroot", "Images");
            }

            await context.MusicalInstruments.AddRangeAsync(instruments);
            await context.SaveChangesAsync();
        }
    }
}
