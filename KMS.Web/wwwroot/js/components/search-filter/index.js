// (function(){
//   function qs(s, r){return (r||document).querySelector(s);}
//   function qsa(s, r){return Array.from((r||document).querySelectorAll(s));}
//   function getCsrf(){ const x = qs("#__afForm input[name='__RequestVerificationToken']"); return x? x.value : ""; }

//   const root = qs("#searchResultRoot");
//   const filterRoot = qs("#filterRoot");
//   if (!root) return;

//   let state = {
//     baseUrl : root.dataset.baseUrl || "/searchpage",
//     q       : root.dataset.initialQ || "",
//     option  : root.dataset.initialOption || "qs",
//     sort    : root.dataset.initialSort || "year_pub:desc",
//     page    : parseInt(root.dataset.initialPage || "1", 10),
//     pageSize: parseInt(root.dataset.initialPagesize || "10", 10),
//     totalPages: parseInt(root.dataset.totalPages || "0", 10),
//     filters : [] // mảng [["bib_type","Book"], ["authors","John"], ...]
//   };

//   function currentFiltersFromDom(){
//     const picked = [];
//     qsa("input.js-filter:checked", filterRoot).forEach(inp=>{
//       const key = inp.getAttribute("data-key");
//       const val = inp.value;
//       if (key && val) picked.push([key, val]);
//     });
//     return picked;
//   }

//   function buildPayload(){
//     const filterBy = currentFiltersFromDom();
//     state.filters = filterBy;
//     return {
//       page: state.page,
//       pageSize: state.pageSize,
//       request: {
//         searchBy: [[ state.option, state.q]],
//         sortBy  : [ state.sort.split(":") ],
//         filterBy: filterBy
//       },
//       type: "quick"
//     };
//   }

//   function buildPagerModel(total, current) {
//     const items = [];
//     const add = (t, o={}) => items.push({ t, ...o });
//     add("nav", { label: "«", page: Math.max(1, current - 1), disabled: current === 1 });

//     if (total <= 7) {
//       for (let i = 1; i <= total; i++) add("page", { page: i, active: i === current });
//     } else {
//       add("page", { page: 1, active: current === 1 });
//       if (current - 3 > 1) add("dots");
//       const start = Math.max(2, current - 2);
//       const end   = Math.min(total - 1, current + 2);
//       for (let i = start; i <= end; i++) add("page", { page: i, active: i === current });
//       if (current + 3 < total) add("dots");
//       add("page", { page: total, active: current === total });
//     }
//     add("nav", { label: "»", page: Math.min(total, current + 1), disabled: current === total });
//     return items;
//   }

//   function renderPagination(pagEl, state) {
//     if (!pagEl) return;
//     const total = Math.max(1, state.totalPages || 1);
//     const items = buildPagerModel(total, state.page);
//     const href  = `${state.baseUrl}?q=${encodeURIComponent(state.q)}&option=${encodeURIComponent(state.option)}`;
//     let html = "";
//     for (const it of items) {
//       if (it.t === "dots") {
//         html += `<li class="page-item disabled"><span class="page-link">…</span></li>`;
//       } else if (it.t === "nav") {
//         const cls = `page-item ${it.disabled ? "disabled" : ""}`;
//         html += `<li class="${cls}"><a class="page-link js-page" href="${href}" data-page="${it.page}" aria-label="${it.label}">${it.label}</a></li>`;
//       } else if (it.t === "page") {
//         const cls = `page-item ${it.active ? "active" : ""}`;
//         html += `<li class="${cls}"><a class="page-link js-page" href="${href}" data-page="${it.page}">${it.page}</a></li>`;
//       }
//     }
//     pagEl.innerHTML = html;
//   }

//   async function renderFragment(){
//     const list = qs("#resultList");
//     const pag  = qs("#pagination");
//     if (!list || !filterRoot) return;

//     list.classList.add("is-loading");

//     const res = await fetch("/searchpage/fragment", {
//       method: "POST",
//       headers: {
//         "Content-Type": "application/json",
//         "RequestVerificationToken": getCsrf()
//       },
//       body: JSON.stringify(buildPayload())
//     });

//     const meta = {
//       totalPages: parseInt(res.headers.get("X-Total-Pages")   || "0", 10),
//       currentPage: parseInt(res.headers.get("X-Current-Page") || state.page, 10),
//       pageSize: parseInt(res.headers.get("X-Page-Size")       || state.pageSize, 10),
//       totalRecords: parseInt(res.headers.get("X-Total-Records") || "0", 10),
//     };

//     const data = await res.json(); // {resultsHtml, filtersHtml}

//     list.innerHTML = data.resultsHtml || "";
//     filterRoot.innerHTML = data.filtersHtml || "";
//     list.classList.remove("is-loading");

//     state.totalPages = Number.isFinite(meta.totalPages) ? meta.totalPages : state.totalPages;
//     state.page       = Number.isFinite(meta.currentPage)? meta.currentPage: state.page;
//     state.pageSize   = Number.isFinite(meta.pageSize)   ? meta.pageSize   : state.pageSize;

//     renderPagination(pag, state);
//   }

//   function pushUrl(){
//     const url = `${state.baseUrl}?q=${encodeURIComponent(state.q)}&option=${encodeURIComponent(state.option)}`;
//     history.pushState({...state}, "", url);
//   }

//   // Pagination (không reload)
//   document.addEventListener("click", function(e){
//     const a = e.target.closest("a.js-page");
//     if (!a) return;
//     e.preventDefault();
//     const p = parseInt(a.dataset.page || "1", 10);
//     if (!Number.isFinite(p)) return;
//     state.page = p;
//     pushUrl();
//     renderFragment();
//   });

//   // Sort
//   document.getElementById("searchSortDropdown")?.addEventListener("click", function(e){
//     const a = e.target.closest(".search-sort__link[data-sort]");
//     if (!a) return;
//     e.preventDefault();
//     state.sort = a.getAttribute("data-sort") || "year_pub:desc";
//     state.page = 1;
//     pushUrl();
//     renderFragment();
//   });

//   // Filter thay đổi (ủy quyền trên #filterRoot vì HTML sẽ thay)
//   document.addEventListener("change", function(e){
//     const inp = e.target.closest("input.js-filter");
//     if (!inp || !filterRoot.contains(inp)) return;
//     state.page = 1;
//     renderFragment();
//   });

//   // Back/Forward
//   window.addEventListener("popstate", function(ev){
//     if (!ev.state) return;
//     state = ev.state;
//     renderFragment();
//   });

//   // Nếu đã có q (redirect lần đầu), đảm bảo pagination chuẩn (SSR đã render), CSR đồng bộ lại
//   if (state.q) {
//     const pag = qs("#pagination");
//     renderPagination(pag, state);
//   }
// })();
