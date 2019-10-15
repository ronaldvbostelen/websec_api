using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Client_tech_resversi_api.Assets.Interfaces;
using Client_tech_resversi_api.Models;
using Client_tech_resversi_api.Models.Principal;

namespace Client_tech_resversi_api.Assets
{
    public class DataSeeder : IDataSeeder
    {
        private readonly ReversiContext _context;
        private readonly IPasswordManager _passwordManager;
        
        public DataSeeder(ReversiContext context, IPasswordManager passwordManager)
        {
            _passwordManager = passwordManager;
            _context = context;
        }
        
        public async Task SeedAsync()
        {
//            _context.Database.EnsureCreated();

            _passwordManager.GenerateSaltAndPasswordHash("1234", out string hash, out string salt);

            var lastChanged = DateTime.Now.ToString();

            if (!_context.Users.Any(x => x.Name == "HelpdeskUser"))
            {
                var helpdeskUser = new User
                {
                    Name = "HelpdeskUser",
                    Email = $"dummy{new Random().Next(0, 100000)}@dummy.com",
                    Password = hash,
                    Salt = salt,
                    UserAccount = new UserAccount
                    {
                        Verified = true,
                        Status = 'A'
                    }
                };

                _context.Add(helpdeskUser);
                await _context.SaveChangesAsync();

                helpdeskUser.UserClaims = new List<UserClaim>
                {
                    new UserClaim
                    {
                        Claim = "UserId",
                        Issuer = "LOCALAUTHORITY",
                        UserId = helpdeskUser.Id,
                        Value = helpdeskUser.Id.ToString()
                    }
                };

                _context.AddRange(helpdeskUser.UserClaims);

                helpdeskUser.UserRoles = new List<UserRole>
                {
                    new UserRole
                    {
                        UserId = helpdeskUser.Id,
                        Role = RoleTypes.User
                    },
                    new UserRole
                    {
                        UserId = helpdeskUser.Id,
                        Role = RoleTypes.Helpdesk
                    }
                };

                _context.AddRange(helpdeskUser.UserRoles);
                await _context.SaveChangesAsync();
            }

            if (!_context.Users.Any(x => x.Name == "AdminUser"))
            {
                var adminUser = new User
                {
                    Name = "AdminUser",
                    Email = $"dummy{new Random().Next(0,100000)}@dummy.com",
                    Password = hash,
                    Salt = salt,
                    UserAccount = new UserAccount
                    {
                        Verified = true,
                        Status = 'A'
                    }
                };

                _context.Add(adminUser);
                await _context.SaveChangesAsync();

                adminUser.UserClaims = new List<UserClaim>
                {
                    new UserClaim
                    {
                        Claim = "UserId",
                        Issuer = "LOCALAUTHORITY",
                        UserId = adminUser.Id,
                        Value = adminUser.Id.ToString()
                    }
                };

                _context.AddRange(adminUser.UserClaims);

                adminUser.UserRoles = new List<UserRole>
                {
                    new UserRole
                    {
                        UserId = adminUser.Id,
                        Role = RoleTypes.User
                    },
                    new UserRole
                    {
                        UserId = adminUser.Id,
                        Role = RoleTypes.Admin
                    }
                };

                _context.AddRange(adminUser.UserRoles);
                await _context.SaveChangesAsync();
            }

            if (!_context.Users.Any(x => x.Name == "SuperAdmin"))
            {
                var adminUser = new User
                {
                    Name = "SuperAdmin",
                    Email = $"dummy{new Random().Next(0, 100000)}@dummy.com",
                    Password = hash,
                    Salt = salt,
                    UserAccount = new UserAccount
                    {
                        Verified = true,
                        Status = 'A'
                    }
                };

                _context.Add(adminUser);
                await _context.SaveChangesAsync();

                adminUser.UserClaims = new List<UserClaim>
                {
                    new UserClaim
                    {
                        Claim = "UserId",
                        Issuer = "LOCALAUTHORITY",
                        UserId = adminUser.Id,
                        Value = adminUser.Id.ToString()
                    }
                };

                _context.AddRange(adminUser.UserClaims);

                adminUser.UserRoles = new List<UserRole>
                {
                    new UserRole
                    {
                        UserId = adminUser.Id,
                        Role = RoleTypes.User
                    },
                    new UserRole
                    {
                        UserId = adminUser.Id,
                        Role = RoleTypes.SuperAdmin
                    }
                };

                _context.AddRange(adminUser.UserRoles);
                await _context.SaveChangesAsync();
            }

            await GenerateRandomUsers(10, hash,salt);

        }

        private async Task GenerateRandomUsers(int amount, string pwHash, string saltHash)
        {
            for (int i = 0; i < amount; i++)
            {
                User randomUser = null;

                while (randomUser == null || _context.Users.Any(x => x.Name == randomUser.Name))
                {
                    randomUser = new User
                    {
                        Name = DogBreeds[new Random().Next(0, DogBreeds.Count)].Replace(' ', '_'),
                        Email = $"dummy{new Random().Next(0, 1000000)}@dummy.com",
                        Password = pwHash,
                        Salt = saltHash,
                        UserAccount = new UserAccount
                        {
                            Verified = true,
                            Status = 'A'
                        }
                    };
                }

                _context.Add(randomUser);
                await _context.SaveChangesAsync();

                _context.Add(new UserClaim
                {
                    Claim = "UserId",
                    Issuer = "LOCALAUTHORITY",
                    UserId = randomUser.Id,
                    Value = randomUser.Id.ToString()
                });

                _context.Add(new UserRole
                {
                    UserId = randomUser.Id,
                    Role = RoleTypes.User
                });

                await _context.SaveChangesAsync();
            }
        }

        private List<string> DogBreeds = new List<string>
        {
            "Affenpinscher",
            "Afghan Hound",
            "Airedale Terrier",
            "Alaskan Malamute",
            "American Staffordshire Bull Terrier",
            "Anatolian Shepherd Dog",
            "Australian Cattle Dog",
            "Australian Kelpie",
            "Australian Shepherd Dog",
            "Australian Silky Terrier",
            "Australian Terrier",
            "Basenji",
            "Basset Fauve de Bretagne",
            "Basset Hound",
            "Beagle",
            "Bearded Collie",
            "Bedlington Terrier",
            "Belgian Shepherd Dog Groenendael",
            "Belgian Shepherd Dog Laekenois",
            "Belgian Shepherd Dog Malinois",
            "Belgian Shepherd Dog Tervueren",
            "Bernese Mountain Dog",
            "Bichon Frise",
            "Bloodhound",
            "Border Collie",
            "Border Terrier",
            "Borzoi",
            "Boston Terrier",
            "Bouvier des Flandres",
            "Boxer",
            "Bracco Italiano",
            "Briard",
            "Brittany",
            "Bull Terrier",
            "Bull Terrier Miniature",
            "Bulldog",
            "Bullmastiff",
            "Cairn Terrier",
            "Cavalier King Charles Spaniel",
            "Cesky Terrier",
            "Chesapeake Bay Retriever",
            "Chihuahua (Smooth Coat)",
            "Chinese Crested",
            "Chow Chow (Smooth)",
            "Clumber Spaniel",
            "Collie (Rough)",
            "Collie (Smooth)",
            "Curly-Coated Retriever",
            "Dachshund (Miniature Long Haired)",
            "Dachshund (Miniature Smooth Haired)",
            "Dachshund (Miniature Wire Haired)",
            "Dachshund (Smooth Haired)",
            "Dachshund (Wire Haired)",
            "Dalmatian",
            "Dandie Dinmont Terrier",
            "Deerhound",
            "Dobermann",
            "Dogue de Bordeaux",
            "English Setter",
            "English Springer Spaniel",
            "English Toy Terrier (Black & Tan)",
            "Field Spaniel",
            "Finnish Lapphund",
            "Finnish Spitz",
            "Flat-Coated Retriever",
            "Fox Terrier Smooth Coat",
            "Fox Terrier Wire Coat",
            "Foxhound",
            "French Bulldog",
            "German Shepherd Dog",
            "German Short-Haired Pointer",
            "German Spitz Klein",
            "German Wire-Haired Pointer",
            "Golden Retriever",
            "Gordon Setter",
            "Great Dane",
            "Greyhound",
            "Harrier Hound",
            "Hungarian Vizsla",
            "Hungarian Wire-Haired Vizsla",
            "Ibizan Hound",
            "Irish Setter",
            "Irish Terrier",
            "Irish Water Spaniel",
            "Irish Wolfhound",
            "Italian Greyhound",
            "Japanese Akita",
            "Japanese Chin",
            "Japanese Spitz",
            "Keeshond",
            "Kerry Blue Terrier",
            "King Charles Spaniel",
            "Labrador Retriever",
            "Lakeland Terrier",
            "Leonberger",
            "Lhaso Apso",
            "Lowchen",
            "Maltese",
            "Manchester Terrier",
            "Maremma Sheepdog",
            "Mastiff",
            "Newfoundland",
            "Norfolk Terrier",
            "Norwich Terrier",
            "Nova Scotia Duck Tolling Retriever",
            "Old English Sheepdog",
            "Papillon",
            "Parson Jack Russell Terrier",
            "Pharaoh Hound",
            "Pointer",
            "Pomeranian",
            "Poodle Miniature",
            "Poodle Standard",
            "Poodle Toy",
            "Portuguese Water Dog",
            "Pug",
            "Pyrenean Mountain Dog",
            "Rhodesian Ridgeback",
            "Rottweiler",
            "Saluki",
            "Samoyed",
            "Schipperke",
            "Schnauzer Giant",
            "Schnauzer Miniature",
            "Schnauzer Standard",
            "Scottish Terrier",
            "Shar Pei",
            "Shetland Sheepdog",
            "Shih Tzu",
            "Siberian Husky",
            "Skye Terrier",
            "Sloughi",
            "Soft Coated Wheaten Terrier",
            "St Bernard",
            "Sussex Spaniel",
            "Swedish Vallhund",
            "Tenterfield Terrier",
            "Tibetan Mastiff",
            "Tibetan Spaniel",
            "Tibetan Terrier",
            "Weimaraner",
            "Welsh Corgi (Cardigan)",
            "Welsh Corgi (Pembroke)",
            "Welsh Springer Spaniel",
            "Welsh Terrier",
            "West Highland White Terrier",
            "Whippet",
            "Yorkshire Terrier"
        };
    }
}
