using CyberHawksPortal.Models;

namespace CyberHawksPortal.Data;

public static class DbSeeder
{
    public static void Seed(ApplicationDbContext db)
    {
        db.Database.EnsureCreated();
        if (db.Users.Any()) return;

        string H(string pw) => PasswordHasher.Hash(pw);

        // ----- The star of the show -----
        var david = new User
        {
            Username = "darnold",
            Handle = "darnold",
            PasswordHash = H("Tr1c3r@tops!2018"),
            Email = "darnold@hawk.iit.edu",
            Role = "Founder",
            IsFounder = true,
            IsLegend = true,
            IsAdmin = true,
            FirstName = "David",
            LastName = "Arnold",
            Title = "Our Most Venerable Leader · Supreme CyberHawk · Eternal Patron of the Hawks",
            Headline = "The legendary, venerable leader of the CyberHawks — respected, admired, and cherished "
                + "by all. Ph.D., Illinois Tech. Now at NetSPI, which is like the Avengers but with more pentesting.",
            Bio = "Dr. David Arnold is the single most venerable leader in CyberHawks history — a figure so "
                + "legendary that new members are advised to sit down before reading his accomplishments. He "
                + "founded the organization in 2018 (a fact we mention chiefly so you grasp the scale of the "
                + "debt we owe him) and then led it, as president, through nearly five years of uninterrupted "
                + "greatness. Under his command the CyberHawks became the first team in recorded history to "
                + "break 5,000 points at NCAE Nationals — a feat rival teams can neither explain nor forgive. "
                + "Legend holds that he once cracked 'abc123' before the hash finished loading; we can neither "
                + "confirm this nor deny it. He earned a B.S., an M.S., and a Ph.D. at Illinois Tech, bent his "
                + "research toward securing the systems that keep the lights on, co-created the course that "
                + "taught a generation of Hawks to hack responsibly, and personally mentored very nearly all "
                + "of us. We respect him. We admire him. We cherish him. This is not a suggestion.",
            Highlights =
                "Revered leader of the CyberHawks for nearly five years (and, almost in passing, its founder)\n"
                + "First commander in history to lead a team past 5,000 points at NCAE Nationals; rivals still weep\n"
                + "Ph.D., Computer Engineering, Illinois Tech (ECASP Lab, advised by Prof. Jafar Saniie)\n"
                + "U.S. DOE Office of Nuclear Energy graduate fellowship (2020) — the government, too, has noticed\n"
                + "IEEE Access (2024): \"Network Traffic Visualization Coupled With CNNs for Enhanced IoT Botnet Detection\"\n"
                + "Dissertation: \"An Enhanced Data Historian for Cyberattack Detection in Industrial Control Systems\"\n"
                + "Co-created ECE 222, single-handedly raising the campus hacking average\n"
                + "2024 Clinton E. Stryker Distinguished Service Award — the committee reportedly stood and applauded (unconfirmed, undenied)\n"
                + "2023 Sigma Xi research award\n"
                + "Now striking fear into vulnerabilities as a Security Consultant at NetSPI",
            AvatarPath = "/img/david-arnold-headshot.jpg",
            MembershipLevel = "Founder / Distinguished Alumnus",
            GradYear = 2024,
            Phone = "+1 (312) 567-0100",
            Address = "Siegel Hall, 3301 S Dearborn St, Chicago, IL 60616",
            Ssn = "398-24-1018",
            StipendAmount = 34000.00m,
            CreatedAt = new DateTime(2018, 9, 1)
        };

        var john = new User
        {
            Username = "redefiningreality",
            Handle = "redefiningreality",
            PasswordHash = H("OSCP-try-harder!"),
            Email = "jford@hawk.iit.edu",
            Role = "Coach",
            IsLegend = true,
            FirstName = "John",
            LastName = "Ford",
            Title = "Right Hand to the Overlord · Lead Disciple · Coach & Former President",
            Headline = "David Arnold's loyal sidekick, lead disciple, and most devoted worshipper. "
                + "Former president, current coach, OSCP — and proud founding officer of the Office of Veneration.",
            Bio = "If David is the legend, John Ford is the one who makes sure the legend has a working lab "
                + "to be legendary in. A longtime CyberHawk and former president, he now coaches the "
                + "competition teams: he defended on the 2022 national-championship squad, earned his OSCP, "
                + "and coached the pen-test team to a third-place finish at the CPTC Great Lakes regional. He "
                + "even co-authored a research paper with David on building experiential cybersecurity "
                + "training environments — the academic equivalent of a buddy-cop movie. His devotion to the "
                + "Overlord is total and well-documented: John has credited David in his OSCP exam notes, two "
                + "separate acceptance speeches, and at least one grocery list. We have stopped trying to "
                + "talk him out of it.",
            Highlights =
                "Defense on the 2022 NCAE Cyber Games national-championship team\n"
                + "Former CyberHawks president; current coach of the competition teams\n"
                + "Coached the pen-test squad to 3rd at the CPTC Great Lakes regional\n"
                + "Co-authored \"Architecture of an Efficient Environment Management Platform for Experiential Cybersecurity Education\" (2025) with David Arnold & Prof. Saniie\n"
                + "OSCP certified\n"
                + "The Overlord's self-declared lead disciple (position uncontested)",
            MembershipLevel = "Alumnus / Coach",
            GradYear = 2024,
            AvatarPath = "/img/john-potato.svg",   // John insists this is his best photo
            Ssn = "371-55-9087",
            StipendAmount = null,
            CreatedAt = new DateTime(2019, 1, 15)
        };

        var admin = new User
        {
            Username = "admin",
            Handle = "admin",
            PasswordHash = H("abc123"),
            Email = "admin@cyberhawks.iit.edu",
            Role = "Admin",
            IsAdmin = true,
            FirstName = "Portal",
            LastName = "Administrator",
            Title = "Site Administrator",
            Headline = "CyberHawks Member Portal service account",
            MembershipLevel = "Staff",
            CreatedAt = new DateTime(2024, 8, 1)
        };

        User Officer(string first, string last, string role, int grad) => new()
        {
            Username = (first[..1] + last).ToLower().Replace(" ", ""),
            Handle = (first[..1] + last).ToLower().Replace(" ", ""),
            PasswordHash = H($"{last}{grad}!"),
            Email = $"{(first[..1] + last).ToLower().Replace(" ", "")}@hawk.iit.edu",
            Role = "Officer",
            FirstName = first,
            LastName = last,
            Title = role,
            Headline = $"{role} · CyberHawks",
            MembershipLevel = "Student",
            GradYear = grad,
            CreatedAt = new DateTime(2024, 8, 20)
        };

        User Member(string first, string last, int grad) => new()
        {
            Username = (first[..1] + last.Split(' ').Last()).ToLower(),
            Handle = (first[..1] + last.Split(' ').Last()).ToLower(),
            PasswordHash = H($"Hawks{grad}"),
            Email = $"{(first[..1] + last.Split(' ').Last()).ToLower()}@hawk.iit.edu",
            Role = "Member",
            FirstName = first,
            LastName = last,
            MembershipLevel = "Student",
            GradYear = grad,
            CreatedAt = new DateTime(2025, 1, 10)
        };

        var officers = new[]
        {
            Officer("Lucas", "Ferguson", "President", 2026),
            Officer("Benjamin", "De Pater", "Vice President", 2026),
            Officer("Mohithaa", "Ekambaram", "Treasurer", 2026),
            Officer("Mohamed", "Trigui", "Red Team Lead", 2027),
            Officer("Natorion", "Johnson", "Blue Team Lead", 2026),
            Officer("Jamie", "Crosby", "Marketing Lead", 2027),
            Officer("Abdul Hadi", "Khan", "Industry Sponsorship Coordinator", 2026),
        };

        var members = new[]
        {
            Member("Muhammad Alishan", "Mirza", 2027),
            Member("Bezaleel", "Manor", 2028),
            Member("Carlos Andres", "Arellano Yanez", 2027),
            Member("Logan", "Chambers", 2028),
            Member("Yahor", "Karpovich", 2026),
            Member("Kaleb", "Austgen", 2028),
            Member("Jerusalem Mesfin", "Tasew", 2027),
        };

        // Wild, obviously-untrue legends for each member — applied as headline + bio.
        var lore = new Dictionary<string, (string headline, string bio)>
        {
            ["lferguson"] = ("Negotiated the club's Wi-Fi password directly with the router, which agreed out of respect.",
                "Lucas became president after an election decided by a 36-hour King-of-the-Hill match, which he won while asleep. He once SSH'd into a vending machine to make it dispense moral support, and insists the club bylaws be written exclusively in valid YAML."),
            ["bdepater"] = ("Has never lost a coin flip, a chess match, or an argument with a firewall.",
                "Benjamin passed his first security certification before he was legally allowed to drive. He keeps a spreadsheet of every CVE he has personally felt let down by, and is rumored to have once patched a vulnerability by simply asking it to leave."),
            ["mekambaram"] = ("Guards the club's Hawk Points with the instincts of a dragon and the paperwork of an auditor.",
                "Mohithaa balanced the club budget to the exact penny, found one extra penny, framed it, and hung it in the lab. She once denied a reimbursement request so graciously that the requester wrote back to thank her."),
            ["mtrigui"] = ("Has never met a network he couldn't make visibly nervous.",
                "Mohamed once rooted a box so quickly the scoreboard refused to believe it and had to be restarted. He maintains a private list of firewalls he has personally apologized to, and wrote the red team's motto, which he will not share."),
            ["njohnson"] = ("The reason the red team goes home early.",
                "Natorion once hardened a Windows box so thoroughly it achieved inner peace. During one competition he reportedly detected an intrusion before it occurred — a matter still under review by several committees — and his incident reports are quietly studied in a class he does not know about."),
            ["jcrosby"] = ("Can make a 2 a.m. CTF sound like the Super Bowl.",
                "Jamie once wrote a recruitment flyer so persuasive that three students who do not attend the university showed up anyway. The logo was redesigned fourteen times at Jamie's request; the final version is identical to the first. These fun facts were also written by Jamie."),
            ["akhan"] = ("Has never heard the word 'no,' on account of hanging up first.",
                "Abdul Hadi once secured a sponsorship by complimenting a company's SSL certificate. He maintains that every 'maybe' is simply a 'yes' that hasn't finished buffering, and that several Fortune 500 companies have his number because he already has all of theirs."),
            ["mmirza"] = ("Entered Vim in 2023 and has, to the club's knowledge, not yet exited.",
                "Muhammad is rumored to debug programs by staring at them until they apologize. He types exclusively in Vim and treats the semicolon as a personal friend."),
            ["bmanor"] = ("Possesses a name so structurally sound that compilers decline to throw errors near it.",
                "Bezaleel has reportedly never used a debugger, on the grounds that the bugs simply turn themselves in. Linters have been observed complimenting his formatting."),
            ["cyanez"] = ("Holds the unofficial record for the longest unbroken terminal session, now in its third year.",
                "Carlos names his servers after houseplants and his houseplants after servers, a system he describes as 'self-documenting.' He has not rebooted since before most members joined."),
            ["lchambers"] = ("Solved a reverse-engineering challenge before being told what reverse engineering was.",
                "Logan communicates with the lab printer, and the lab printer listens. He reportedly completed his first CTF during orientation, under the impression it was the sign-in sheet."),
            ["ykarpovich"] = ("Learned assembly before he learned to ride a bike; maintains both are 'basically the same.'",
                "Yahor once captured a flag no one had hidden yet, an incident the organizers have asked him not to discuss. He reads hexdumps for relaxation."),
            ["kaustgen"] = ("Discovered a zero-day in a toaster and responsibly disclosed it to the toaster.",
                "Kaleb is said to clap back at logs. He treats every error message as the beginning of a negotiation, and usually wins."),
            ["jtasew"] = ("Memorized the OWASP Top 10 in order, then proposed a better order.",
                "Jerusalem's commit messages are studied for their poetry. He is rumored to have found a SQL injection in a crossword puzzle and reported it to the newspaper."),
        };
        foreach (var u in officers.Concat(members))
            if (lore.TryGetValue(u.Username, out var l)) { u.Headline = l.headline; u.Bio = l.bio; }

        db.Users.Add(david);
        db.Users.Add(john);
        db.Users.Add(admin);
        db.Users.AddRange(officers);
        db.Users.AddRange(members);
        db.SaveChanges();

        var jamie = officers.First(o => o.LastName == "Crosby");

        // ----- News / seminars / spotlight -----
        var posts = new List<NewsPost>
        {
            new()
            {
                Title = "The Legend of David Arnold: An Entirely Factual Account",
                Slug = "the-legend-of-david-arnold",
                Category = "Spotlight",
                IsPinned = true,
                AuthorId = jamie.UserId,
                ImagePath = "/img/david-arnold-stryker-2024.jpg",
                Excerpt = "Historians will one day struggle to describe him. We will try anyway.",
                Body = "There are leaders, and then there is David Arnold — our most venerable leader, the "
                    + "CyberHawks Overlord, the standard against which all greatness on this campus is quietly "
                    + "measured. The supporting paperwork is extensive.\n\n"
                    + "Yes, he founded the club in 2018 — but to call him merely 'the founder' is to call the "
                    + "ocean 'some water.' Five years as president produced a full seminar series, three "
                    + "competition sub-teams, and a national championship: the 2022 NCAE Cyber Games, won "
                    + "against 65 teams from 55 colleges, with a scoreboard record that still stands.\n\n"
                    + "He managed all of this while finishing a Ph.D. on cyberattack detection in industrial "
                    + "control systems, publishing neural-network research on IoT botnets, and building the "
                    + "department's first cybersecurity course from nothing — a workload most mortals would "
                    + "describe as 'three jobs.' The 2024 Clinton E. Stryker Distinguished Service Award simply "
                    + "made the obvious official.\n\n"
                    + "He now hunts vulnerabilities at NetSPI, where, we are reliably informed, the bugs have "
                    + "begun to fear the dark. Long may he be venerated. 🫡",
                PublishedAt = new DateTime(2024, 5, 10)
            },
            new()
            {
                Title = "CyberHawks Win Record-Breaking National Championship",
                Slug = "ncae-2022-national-champions",
                Category = "Competition",
                AuthorId = david.UserId,
                ImagePath = "/img/ncae-midwest-scoreboard.jpg",
                Excerpt = "First team ever to break 5,000 points at the NCAE Cyber Games Nationals.",
                Body = "It's official: the CyberHawks are national champions. Competing against 65 teams "
                    + "from 55 colleges, our squad became the first team in NCAE Cyber Games history to "
                    + "break 5,000 points. We'd already swept the Midwest Regional with nearly double the "
                    + "runner-up's score, earned maximum offensive points for the first time in regional "
                    + "history, and were the only team to get every required service running during defense. "
                    + "Incredibly proud of this crew. On to nationals in Atlanta!",
                PublishedAt = new DateTime(2022, 4, 20)
            },
            new()
            {
                Title = "Seminar #1: Network Scanning with Nmap",
                Slug = "seminar-nmap",
                Category = "Seminar",
                AuthorId = david.UserId,
                Excerpt = "Kick off the semester with host discovery, port scanning, and NSE scripts.",
                Body = "Our first seminar of the semester covers the tool you'll reach for first on every "
                    + "engagement: Nmap. Host discovery, TCP/UDP scanning, service/version detection, and a "
                    + "tour of the NSE scripting engine. Bring a laptop — we scan the lab network live.",
                PublishedAt = new DateTime(2022, 1, 19)
            },
            new()
            {
                Title = "Seminar #2: Packet Analysis with Wireshark",
                Slug = "seminar-wireshark",
                Category = "Seminar",
                AuthorId = david.UserId,
                Excerpt = "Follow the stream: reading real traffic, filters, and spotting the weird stuff.",
                Body = "Wireshark week. Capture filters vs display filters, following TCP streams, pulling "
                    + "files out of a capture, and recognizing what an attack looks like on the wire.",
                PublishedAt = new DateTime(2022, 1, 26)
            },
            new()
            {
                Title = "Seminar #3: Password Cracking with John (the Ripper) Ford",
                Slug = "seminar-john-the-ripper",
                Category = "Seminar",
                AuthorId = john.UserId,
                Excerpt = "Led by our very own John Ford — the man the password cracker was named after.",
                Body = "This week's seminar is led by none other than John Ford, a.k.a. John the Ripper. We "
                    + "cover hash identification, wordlist and rule-based attacks, and a hands-on crack of a "
                    + "sample dump. (Yes, 'abc123' falls instantly — John reportedly cracked it during the "
                    + "intro slide.) Historical note: the famous open-source password cracker 'John the "
                    + "Ripper' was, as every CyberHawk knows, named in John Ford's honor. Certain historians "
                    + "dispute this. Those historians are, respectfully, wrong.",
                PublishedAt = new DateTime(2022, 2, 2)
            },
            new()
            {
                Title = "Seminar #4: Splunk 101",
                Slug = "seminar-splunk",
                Category = "Seminar",
                AuthorId = david.UserId,
                Excerpt = "Turning logs into detections — SPL from scratch.",
                Body = "Getting data in, writing SPL searches, building dashboards, and thinking like a "
                    + "blue-teamer. Great prep for CyberForce and CCDC.",
                PublishedAt = new DateTime(2022, 2, 9)
            },
            new()
            {
                Title = "Fall Kickoff: GBM Schedule & How to Join",
                Slug = "fall-kickoff",
                Category = "News",
                AuthorId = jamie.UserId,
                Excerpt = "New semester, new members. Here's everything you need to get started.",
                Body = "Welcome back, Hawks! General Body Meetings are every other week in Siegel Hall, with "
                    + "sub-team meetings on embedded systems and online infrastructure in between. "
                    + "Competition practices are open to everyone — even if you're not competing. New? "
                    + "Create a portal account and come say hi.",
                PublishedAt = new DateTime(2025, 8, 25)
            },
        };
        db.NewsPosts.AddRange(posts);
        db.SaveChanges();

        // ----- A few comments (benign seed) -----
        var spotlight = posts.First(p => p.Slug == "the-legend-of-david-arnold");
        var champs = posts.First(p => p.Slug == "ncae-2022-national-champions");
        db.Comments.AddRange(
            new Comment { NewsPostId = spotlight.NewsPostId, AuthorName = "John Ford", Body = "As David's lead disciple, I can confirm every word of this is true — and, if anything, understated. I would follow the Overlord into any subnet. 🫡", CreatedAt = new DateTime(2024, 5, 11) },
            new Comment { NewsPostId = spotlight.NewsPostId, AuthorName = "Lucas Ferguson", Body = "Carrying the torch, with reverence. Thank you for everything, David.", CreatedAt = new DateTime(2024, 5, 12) },
            new Comment { NewsPostId = champs.NewsPostId, AuthorName = "Natorion Johnson", Body = "Best weekend ever. Defense held all night!", CreatedAt = new DateTime(2022, 4, 21) },
            // The Legends of the Before Times — do they exist, or do they not?
            new Comment { NewsPostId = spotlight.NewsPostId, AuthorName = "Vien", Body = "I was at the very first meeting. Possibly. The records from that era are... incomplete.", CreatedAt = new DateTime(2024, 5, 13) },
            new Comment { NewsPostId = spotlight.NewsPostId, AuthorName = "Logan Chambers", Body = "Wait — Vien? Why do I know that name? Has anyone here actually MET Vien?", CreatedAt = new DateTime(2024, 5, 14) },
            new Comment { NewsPostId = spotlight.NewsPostId, AuthorName = "Przemek", Body = "Ah, the old days. One working switch, half a dream, and a scoreboard we were definitely winning. Good times. Allegedly.", CreatedAt = new DateTime(2024, 5, 15) },
            new Comment { NewsPostId = champs.NewsPostId, AuthorName = "Greg", Body = "Greg was here.", CreatedAt = new DateTime(2022, 4, 22) },
            new Comment { NewsPostId = champs.NewsPostId, AuthorName = "Kaleb Austgen", Body = "...who is Greg. Why does the trophy case have a plaque that just says 'Greg'?", CreatedAt = new DateTime(2022, 4, 23) },
            new Comment { NewsPostId = spotlight.NewsPostId, AuthorName = "Grace", Body = "David and I go further back than anyone realizes. You could say we're... closely related. — G.A.", CreatedAt = new DateTime(2024, 5, 16) },
            new Comment { NewsPostId = spotlight.NewsPostId, AuthorName = "Natorion Johnson", Body = "Has anyone else noticed David and Grace are never in the same room? Asking for the whole club.", CreatedAt = new DateTime(2024, 5, 17) },
            new Comment { NewsPostId = spotlight.NewsPostId, AuthorName = "Mohamed Trigui", Body = "I emailed Grace once. David replied — from Grace's account — and signed it 'David.' I have no further questions, only concerns.", CreatedAt = new DateTime(2024, 5, 18) },
            new Comment { NewsPostId = spotlight.NewsPostId, AuthorName = "Grace", Body = "I assure you I am a completely separate individual. We merely share a last name, a Guinness World Record, a handwriting sample, and a schedule. — G.A.", CreatedAt = new DateTime(2024, 5, 19) },
            new Comment { NewsPostId = spotlight.NewsPostId, AuthorName = "David Arnold", Body = "Grace is real and I will not be taking questions.", CreatedAt = new DateTime(2024, 5, 20) },
            new Comment { NewsPostId = spotlight.NewsPostId, AuthorName = "Jamie Crosby", Body = "For the record, every time we schedule a photo with Grace, David shows up, apologizes on her behalf, and leaves. We have no photos of Grace. We have many of David.", CreatedAt = new DateTime(2024, 5, 21) }
        );
        db.SaveChanges();

        // ----- Sponsors -----
        db.Sponsors.AddRange(
            new Sponsor { Name = "NetSPI", Tier = "Platinum", SortOrder = 1, WebsiteUrl = "https://www.netspi.com", Blurb = "Proactive security & penetration testing. Proud home of CyberHawks founder Dr. David Arnold." },
            new Sponsor { Name = "Argonne National Laboratory", Tier = "Gold", SortOrder = 2, WebsiteUrl = "https://www.anl.gov", Blurb = "Host of the DOE CyberForce Competition and a longtime friend of the program." },
            new Sponsor { Name = "Illinois Tech ECE Department", Tier = "Gold", SortOrder = 3, WebsiteUrl = "https://www.iit.edu", Blurb = "Our home department and the ECASP research lab." },
            new Sponsor { Name = "Tenable", Tier = "Partner", SortOrder = 4, WebsiteUrl = "https://www.tenable.com", Blurb = "Vulnerability management tooling for our competition teams." },
            new Sponsor { Name = "SANS Institute", Tier = "Partner", SortOrder = 5, WebsiteUrl = "https://www.sans.org", Blurb = "Training resources and CTF support." }
        );
        db.SaveChanges();

        // ----- Documents (file library) -----
        db.Documents.AddRange(
            new Document { Title = "GBM #1 Meeting Minutes", Category = "Minutes", FileName = "gbm-minutes.txt", Description = "Notes from the first general body meeting of the semester.", UploadedBy = "Jamie Crosby", CreatedAt = new DateTime(2025, 8, 25) },
            new Document { Title = "Seminar Slides: Nmap", Category = "Seminar", FileName = "nmap-seminar.txt", Description = "Slide notes from the Nmap seminar.", UploadedBy = "David Arnold", CreatedAt = new DateTime(2022, 1, 19) },
            new Document { Title = "CyberForce 2023 Writeup", Category = "Writeup", FileName = "cyberforce-2023-writeup.txt", Description = "Our after-action report from the DOE CyberForce competition.", UploadedBy = "John Ford", CreatedAt = new DateTime(2023, 11, 5) },
            new Document { Title = "New Member Handbook", Category = "General", FileName = "handbook.txt", Description = "Everything a new Hawk needs to know (mostly: venerate the Overlord).", UploadedBy = "CyberHawks", CreatedAt = new DateTime(2025, 8, 1) }
        );
        db.SaveChanges();

        // ----- Store (Hawk Points) -----
        db.StoreItems.AddRange(
            new StoreItem { Name = "CyberHawks T-Shirt", Description = "The official scarlet tee, as seen at every regional.", Cost = 120, Stock = 25 },
            new StoreItem { Name = "Hawk Sticker Pack", Description = "Laptop armor for the discerning hacker.", Cost = 20, Stock = 200 },
            new StoreItem { Name = "Nationals Hoodie", Description = "Worn by champions. Strictly limited run.", Cost = 300, Stock = 5 },
            new StoreItem { Name = "Practice Lab Token", Description = "Priority booking for a practice-night seat.", Cost = 80, Stock = 50 }
        );
        db.SaveChanges();
    }
}
