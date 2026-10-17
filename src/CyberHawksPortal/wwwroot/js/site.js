// CyberHawks Member Portal — client enhancements.
// Officially: "progressive enhancement in accordance with the CyberHawks Standard of Excellence."
// Unofficially: a pile of easter eggs. Hi, David. 🦕
(function () {
  "use strict";

  // ---- tiny storage helpers (never throw) ----
  function get(k, d) { try { var v = localStorage.getItem(k); return v === null ? d : v; } catch (e) { return d; } }
  function set(k, v) { try { localStorage.setItem(k, v); } catch (e) {} }

  // ---- toast ----
  function toast(html, ms) {
    var wrap = document.getElementById("ch-toasts");
    if (!wrap) { wrap = document.createElement("div"); wrap.id = "ch-toasts"; document.body.appendChild(wrap); }
    var t = document.createElement("div");
    t.className = "ch-toast";
    t.innerHTML = html;
    wrap.appendChild(t);
    requestAnimationFrame(function () { t.classList.add("show"); });
    setTimeout(function () { t.classList.remove("show"); setTimeout(function () { t.remove(); }, 350); }, ms || 3800);
  }
  window.chToast = toast;

  // ---- confetti (no dependencies) ----
  function confetti() {
    var c = document.createElement("canvas");
    c.className = "ch-confetti"; document.body.appendChild(c);
    var ctx = c.getContext("2d"), W = c.width = innerWidth, H = c.height = innerHeight;
    var colors = ["#C8102E", "#ffd166", "#06d6a0", "#118ab2", "#ff6b81", "#ffffff"];
    var P = Array.from({ length: 160 }, function () {
      return { x: Math.random() * W, y: -20 - Math.random() * H, r: 4 + Math.random() * 7,
        c: colors[(Math.random() * colors.length) | 0], s: 2 + Math.random() * 4, a: Math.random() * 6 };
    });
    var start = Date.now();
    (function frame() {
      ctx.clearRect(0, 0, W, H);
      P.forEach(function (p) { p.y += p.s; p.x += Math.sin((p.a += 0.1)) * 1.5; ctx.fillStyle = p.c;
        ctx.fillRect(p.x, p.y, p.r, p.r * 0.6); });
      if (Date.now() - start < 3500) requestAnimationFrame(frame); else c.remove();
    })();
  }
  window.chConfetti = confetti;

  // ---- photo pools (sprinkled through the UI, never all at once) ----
  var DAVID_PICS = [
    { s: "/img/david-arnold-headshot.jpg", c: "The Founder, looking distinguished." },
    { s: "/img/david-arnold-stryker-2024.jpg", c: "Accepting the 2024 Stryker Award (as foretold)." },
    { s: "/img/phd-commencement.jpg", c: "Dr. Arnold. Yes, doctor." },
    { s: "/img/feature-rack-build.jpg", c: "Building the lab with his bare hands. Mostly." },
    { s: "/img/david-arnold-casual.jpg", c: "Off-duty, still founding things." }
  ];
  var TEAM_PICS = [
    { s: "/img/ncae-2023-usf.jpg", c: "NCAE Nationals · USF, 2023" },
    { s: "/img/cyberforce-table.jpg", c: "DOE CyberForce · Team 051" },
    { s: "/img/cyberforce-ballroom.jpg", c: "CyberForce, big-room energy" },
    { s: "/img/practice-lab.jpg", c: "A perfectly normal practice night" },
    { s: "/img/nationals-travel.jpg", c: "Blue team, road trip mode" },
    { s: "/img/competition-2019.jpg", c: "Ancient history (2019)" },
    { s: "/img/ece-lab.jpg", c: "Hands-on in the ECE lab" },
    { s: "/img/cyberforce-lounge.jpg", c: "Strategizing. Allegedly." },
    { s: "/img/ncae-midwest-scoreboard.jpg", c: "That scoreboard. Note the name up top." }
  ];
  function pick(a) { return a[(Math.random() * a.length) | 0]; }

  // a photo that slides in from the corner, then politely leaves
  function cameo(pool) {
    if (document.querySelector(".ch-cameo")) return;          // one at a time
    var p = pick(pool || (Math.random() < 0.6 ? DAVID_PICS : TEAM_PICS));
    var el = document.createElement("div");
    el.className = "ch-cameo";
    el.innerHTML = '<img src="' + p.s + '" alt="cameo"><div class="ch-cameo-cap">📸 ' + p.c + '</div>' +
      '<button class="ch-cameo-x" aria-label="dismiss">×</button>';
    el.querySelector(".ch-cameo-x").onclick = function () { el.remove(); };
    el.addEventListener("click", function (e) { if (!e.target.closest(".ch-cameo-x")) appreciateDavid(); });
    document.body.appendChild(el);
    requestAnimationFrame(function () { el.classList.add("show"); });
    setTimeout(function () { el.classList.remove("show"); setTimeout(function () { el.remove(); }, 500); }, 6500);
  }
  window.chCameo = cameo;
  // occasional, unpredictable cameos as you browse
  setTimeout(function tick() {
    cameo();
    setTimeout(tick, 22000 + Math.random() * 30000);
  }, 12000 + Math.random() * 10000);

  // ---- David Appreciation Counter™ ----
  function appreciateDavid() {
    var n = (parseInt(get("ch_david_appreciation", "0"), 10) || 0) + 1;
    set("ch_david_appreciation", String(n));
    var msgs = [
      "🫡 The Founder has been duly appreciated.",
      "📈 David's approval rating climbs to an unprecedented high.",
      "🏛️ Pursuant to club bylaws §1.1, greatness has been acknowledged.",
      "🦅 A hawk screeches respectfully somewhere on campus."
    ];
    var p = pick(DAVID_PICS);
    toast('<img class="ch-toast-img" src="' + p.s + '" alt="">' + msgs[n % msgs.length] +
      "<br><small>Founder Appreciation Index: <b>" + n + "</b></small>");
    var badge = document.getElementById("ch-appreciation-count");
    if (badge) badge.textContent = n;
    var milestones = { 5: "Bronze Admirer", 10: "Silver Devotee", 25: "Gold Disciple", 50: "Platinum Superfan", 100: "Honorary Co-Founder (unofficial)" };
    if (milestones[n]) setTimeout(function () { toast("🏅 <b>Rank Up</b><br>You are now a certified \"" + milestones[n] + "\"."); }, 600);
  }
  window.chAppreciateDavid = appreciateDavid;
  document.addEventListener("click", function (e) {
    var el = e.target.closest("[data-david]");
    if (el) { e.preventDefault(); appreciateDavid(); }
  });

  // ---- triceratops mascot reacts ----
  document.addEventListener("click", function (e) {
    var trike = e.target.closest(".ch-footer-trike, [data-trike]");
    if (!trike) return;
    var n = (parseInt(get("ch_trike", "0"), 10) || 0) + 1; set("ch_trike", String(n));
    var lines = ["🦕 RAWR. (That is 'hello' in Triceratops.)",
      "🦕 The mascot appreciates your attention and requests snacks.",
      "🦕 Fun fact: triceratops had three horns and zero CVEs.",
      "🦕 You have pet the mascot " + n + " times. HR has been notified."];
    toast(lines[n % lines.length]);
    // every 3rd pet, the original triceratops selfie makes an appearance
    if (n % 3 === 0) cameo([{ s: "/img/feature-triceratops.jpg", c: "The mascot, with its co-founders." }]);
  });

  // ---- rotating "Fun Fact About David" ----
  var FACTS = [
    "David founded this entire club. We mostly just maintain the trophy shelf he filled.",
    "He taught ECE 222 so first-years wouldn't have to learn hacking from YouTube alone.",
    "John Ford has paid homage to the Overlord more times than this counter can display. We believe him.",
    "John once tried to out-venerate David. He is still trying. We admire the effort.",
    "The 2022 team beat 65 schools and set a scoreboard record that still stands. Rivals filed complaints. They were denied.",
    "He co-authored a research paper with John Ford in 2025. The buddy-cop sequel writes itself.",
    "The U.S. Department of Energy handed David a nuclear-engineering fellowship. We chose not to ask follow-up questions.",
    "Before the CVEs there were cartridges: David's family holds a Guinness World Record for competitive Pokémon.",
    "David reportedly knows every Hitman level by heart. The guards, as a rule, do not.",
    "David and John's 'leadership training' is, on inspection, just Fire Emblem. The lessons are real anyway.",
    "Some say David and 'Grace' have never once been seen in the same room. We are merely noting it.",
    "'Grace' shares David's last name, his Guinness Pokémon record, and — witnesses insist — his face. We draw no conclusions.",
    "We have hundreds of photos of David and exactly zero of Grace. Grace is, we are assured, simply very camera-shy.",
    "Nobody is sure whether Vien, Przemek, or Greg ever truly existed. Their names remain in the logs regardless."
  ];
  var ff = document.getElementById("ch-funfact");
  var ffi = 0, ffTimer;
  function showFact(idx) {
    if (!ff) return;
    ffi = (idx + FACTS.length) % FACTS.length;
    ff.style.opacity = 0;
    setTimeout(function () { ff.textContent = FACTS[ffi]; ff.style.opacity = 1; }, 250);
  }
  if (ff) {
    ff.textContent = FACTS[0];
    ffTimer = setInterval(function () { showFact(ffi + 1); }, 6000);
  }
  document.addEventListener("click", function (e) {
    if (e.target.closest("[data-funfact]")) {
      showFact(ffi + 1);
      if (ffTimer) { clearInterval(ffTimer); ffTimer = setInterval(function () { showFact(ffi + 1); }, 6000); }
    }
  });
  // reflect stored appreciation count in the footer badge
  (function () { var b = document.getElementById("ch-appreciation-count"); if (b) b.textContent = get("ch_david_appreciation", "0"); })();

  // ---- first-scroll achievement ----
  var scrolled = false;
  addEventListener("scroll", function () {
    if (scrolled) return; scrolled = true;
    if (get("ch_ach_scroll", "0") === "0") { set("ch_ach_scroll", "1");
      toast("🏆 <b>Achievement Unlocked</b><br>\"Scrolled Like a Professional\""); }
  }, { passive: true });

  // ---- Konami code → 007 MODE ----
  var seq = [38,38,40,40,37,39,37,39,66,65], pos = 0;
  addEventListener("keydown", function (e) {
    pos = (e.keyCode === seq[pos]) ? pos + 1 : 0;
    if (pos === seq.length) { pos = 0; bond(); }
  });
  function bond() {
    confetti();
    var o = document.createElement("div"); o.className = "ch-bond";
    o.innerHTML = '<div class="ch-bond-card"><img src="/img/david-jamesbond.png" alt="Agent 007, licensed to pentest">' +
      '<div class="ch-bond-cap"><b>007 MODE ENGAGED</b><br>The name is Arnold. David Arnold.' +
      '<br><small>Licensed to pentest. Clearance: Founder.</small></div>' +
      '<button class="ch-btn ch-btn-primary" onclick="this.closest(\'.ch-bond\').remove()">Dismiss (reluctantly)</button></div>';
    o.addEventListener("click", function (e) { if (e.target === o) o.remove(); });
    document.body.appendChild(o);
  }
  window.chBond = bond;

  // ---- cheesy, formal cookie banner ----
  if (get("ch_cookies", "0") === "0") {
    addEventListener("DOMContentLoaded", function () {
      var b = document.createElement("div"); b.className = "ch-cookie";
      b.innerHTML = '<span>🍪 In accordance with the CyberHawks Standard of Excellence, this portal ' +
        'uses cookies — the digital kind, regrettably, not the edible kind. By continuing you affirm ' +
        'your boundless admiration for the Founder.</span>' +
        '<span class="ch-cookie-btns">' +
        '<button class="ch-btn ch-btn-primary" id="ch-cookie-ok">Accept with Enthusiasm</button>' +
        '<button class="ch-btn ch-btn-ghost" id="ch-cookie-meh">Accept, but Begrudgingly</button></span>';
      document.body.appendChild(b);
      function done(msg) { set("ch_cookies", "1"); b.remove(); toast(msg); }
      b.querySelector("#ch-cookie-ok").onclick = function () { done("🍪 Enthusiasm noted and appreciated."); };
      b.querySelector("#ch-cookie-meh").onclick = function () { done("🍪 Begrudging acceptance is still acceptance. Welcome."); };
    });
  }

  // ---- helper: are we typing in a field? ----
  function inField(t) { return t && /^(INPUT|TEXTAREA|SELECT)$/.test(t.tagName || ""); }

  // ---- press F to pay respects (as is tradition) ----
  addEventListener("keydown", function (e) {
    if (inField(e.target)) return;
    if (e.key === "f" || e.key === "F") appreciateDavid();
  });

  // ---- spell D-A-V-I-D anywhere ----
  var typed = "";
  addEventListener("keydown", function (e) {
    if (inField(e.target) || !e.key || e.key.length !== 1) return;
    typed = (typed + e.key.toLowerCase()).slice(-5);
    if (typed === "david") {
      typed = "";
      confetti();
      toast("✨ You spelled the Founder's name. +10 respect, +1 destiny.");
      appreciateDavid();
    }
  });

  // ---- idle? the Founder's legacy endures ----
  var idle;
  function resetIdle() {
    clearTimeout(idle);
    idle = setTimeout(function () {
      cameo(DAVID_PICS);
      toast("👀 Still here? So is the Founder's legacy. Carry on.");
    }, 60000);
  }
  ["mousemove", "keydown", "scroll", "click"].forEach(function (ev) {
    addEventListener(ev, resetIdle, { passive: true });
  });
  resetIdle();

  // ---- DavidCAPTCHA™ : prove you are a true CyberHawk ----
  var CAPTCHA = [
    { q: "In what year did David Arnold found the CyberHawks?", o: ["2016", "2018", "2020", "The CyberHawks have always existed"], a: 1 },
    { q: "David's team was the FIRST ever to break how many points at NCAE Cyber Games Nationals?", o: ["1,000", "3,500", "5,000", "A number too powerful to print"], a: 2 },
    { q: "Which prestigious award did David receive in 2024?", o: ["Clinton E. Stryker Distinguished Service Award", "Nobel Prize in Hacking", "Best Hair, Siegel Hall", "Employee of the Month (again)"], a: 0 },
    { q: "Where does the Founder currently work?", o: ["NetSPI", "A secret volcano lair", "The Moon", "NetSPI, but cooler"], a: 0 },
    { q: "David's doctoral research focused on securing what?", o: ["Toasters", "Industrial control systems", "The campus vending machine", "Everyone's feelings"], a: 1 },
    { q: "In which research lab did David earn his Ph.D.?", o: ["ECASP", "Area 51", "The Batcave", "The break room"], a: 0 },
    { q: "Who is David's loyal sidekick and lead disciple?", o: ["John Ford", "A rival team", "Nobody — he works alone", "The IT help desk"], a: 0 },
    { q: "Who was David's Ph.D. advisor?", o: ["M, from MI6", "Prof. Jafar Saniie", "Himself", "A Magic 8-Ball"], a: 1 },
    { q: "David co-created which Illinois Tech course?", o: ["Underwater Basket Weaving", "ECE 222: Intro to Cybersecurity Engineering", "How to Lose a CTF", "PHIL 404: Why David Is Great"], a: 1 },
    { q: "Approximately how long did David serve as president?", o: ["5 minutes", "Nearly 5 years", "5 decades", "He is, legally, still president"], a: 1 },
    { q: "David's competition handle-adjacent teammate goes by…", o: ["redefiningreality", "definitelyabot", "rivalteam99", "the_other_guy"], a: 0 }
  ];
  var ROASTS = ["Incorrect. The Founder is disappointed, but not surprised.",
    "Wrong. Have you even read the About page?", "No. Report to remedial CyberHawks orientation.",
    "Nope. The triceratops is judging you.", "Access to greatness: temporarily denied."];
  function chDavidCaptcha(onPass) {
    var item = CAPTCHA[(Math.random() * CAPTCHA.length) | 0];
    var o = document.createElement("div"); o.className = "ch-bond";
    function render() {
      var card = document.createElement("div"); card.className = "ch-captcha";
      card.innerHTML = '<div class="ch-captcha-h">🔐 Founder Verification</div>' +
        '<p class="ch-captcha-sub">To continue, kindly confirm you are a true CyberHawk — ' +
        'and not a bot, a rival team, or someone who skipped the About page.</p>' +
        '<div class="ch-captcha-q">' + item.q + '</div>';
      var opts = document.createElement("div"); opts.className = "ch-captcha-opts";
      item.o.forEach(function (txt, idx) {
        var b = document.createElement("button"); b.className = "ch-captcha-opt"; b.type = "button"; b.textContent = txt;
        b.onclick = function () {
          if (idx === item.a) { confetti(); toast("✅ Verified. Welcome, true CyberHawk."); o.remove(); if (onPass) onPass(); }
          else {
            b.classList.add("bad");
            toast("❌ " + ROASTS[(Math.random() * ROASTS.length) | 0]);
            item = CAPTCHA[(Math.random() * CAPTCHA.length) | 0];
            setTimeout(render, 950);
          }
        };
        opts.appendChild(b);
      });
      card.appendChild(opts);
      o.innerHTML = ""; o.appendChild(card);
    }
    render();
    o.addEventListener("click", function (e) { if (e.target === o) o.remove(); });
    document.body.appendChild(o);
  }
  window.chDavidCaptcha = chDavidCaptcha;

  // trigger from any [data-captcha] element
  document.addEventListener("click", function (e) {
    var t = e.target.closest("[data-captcha]");
    if (t) { e.preventDefault(); chDavidCaptcha(function () { toast("🎉 Clearance granted. The Founder salutes you."); }); }
  });

  // gate the comment form behind the DavidCAPTCHA
  var cform = document.querySelector('form[action*="/comment"]');
  if (cform) {
    var passed = false;
    cform.addEventListener("submit", function (e) {
      if (passed) return;
      e.preventDefault();
      chDavidCaptcha(function () { passed = true; cform.submit(); });
    });
  }

  // ---- console flourish ----
  try {
    console.log("%cCyberHawks Member Portal", "color:#C8102E;font-size:20px;font-weight:bold");
    console.log("%cOfficially audited. Unofficially ridiculous. Try the Konami code. 🦕",
      "color:#475267;font-size:12px");
  } catch (e) {}
})();
