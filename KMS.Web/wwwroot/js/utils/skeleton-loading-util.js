var skeletonCardHeader =
    '<div class="search-page__result-header">' +
    '   <div class="search-page__result-header-filter skeleton-box" style="width: 10rem; height: 16px">' +
    "       <label></label>" +
    "   </div>" +
    '	<div class="search-page__result-header-left" style="width: 20rem;">' +
    '		<p class="search-page__result-title skeleton-line"></p>' +
    '			<p class="search-page__no-result-header-text skeleton-line">' +
    "			</p>" +
    "	</div>" +
    '	<div class="search-page__result-header-right gap-0" style="height: 16px;">' +
    '		<label class="search-page__filter-label skeleton-line" style="width: 10rem; height: 100%"></label>' +
    '		<div class="search-sort dropdown skeleton-box" style="width: 15rem; height: 100%">' +
    "		</div>" +
    "	</div>" +
    '	<div class="search-page__result-header-horizontal-line"></div>' +
    "</div>";

var skeletonCard =
    '<div class="search-page__result-item">' +
    '    <div class="gridDocuments__documentCard">' +
    '        <div class="documentCard__info-with-thumbnail">' +
    '            <div class="documentCard__image-action-wrapper">' +
    "                <div" +
    '                    class="documentCard__image-wrapper skeleton-box"' +
    "                >" +
    "                </div>" +
    '                <div class="documentCard__actions--mobile">' +
    "                    <div" +
    '                        class="documentCard__qr-wrapper skeleton-box"' +
    '                        style="position: relative"' +
    "                    >" +
    "                    </div>" +
    '                    <div class="documentCard__save-wrapper border-0 skeleton-box">' +
    "                    </div>" +
    "                </div>" +
    "            </div>" +
    '            <div class="documentCard__info">' +
    '                <a class="documentCard__info-title title skeleton-line">' +
    "                </a>" +
    '                <p class="authors skeleton-line">' +
    "                </p>" +
    '                <p class="skeleton-line"></p>' +
    '                <p class="skeleton-line"></p>' +
    '                <p class="skeleton-line"></p>' +
    '                <p class="documentCard__info-summary paragraph skeleton-line">' +
    "                </p>" +
    "            </div>" +
    '            <div class="documentCard__actions">' +
    "                <div" +
    '                    class="documentCard__qr-wrapper skeleton-box"' +
    '                    style="position: relative"' +
    "                >" +
    "                </div>" +
    '                <div class="documentCard__save-wrapper border-0 skeleton-box">' +
    "                </div>" +
    "            </div>" +
    "        </div>" +
    '        <div class="documentCard__footer bg-transparent">' +
    '            <div class="documentCard__category-info skeleton-box">' +
    "            </div>" +
    '            <div class="documentCard__source-info skeleton-box">' +
    "            </div>" +
    '            <div class="documentCard__digital-link skeleton-box">' +
    "            </div>" +
    "        </div>" +
    "    </div>" +
    "</div>";

export const skeletonDocumentCardList =
    skeletonCardHeader +
    '<div class="search-page__result-list">' +
    Array.from({ length: 10 })
        .map(() => skeletonCard)
        .join("") +
    "</div>";

const facetSkeletonItem =
    '<div class="search-filter__group border-bottom pb-2 skeleton-box">' +
    "    <button" +
    '        class="search-filter__toggle btn w-100 d-flex justify-content-between align-items-center p-0 collapsed"' +
    '        type="button"' +
    '        data-bs-toggle="collapse"' +
    '        data-bs-target="#facet-filter-tenants"' +
    '        aria-expanded="false"' +
    "    ></button>" +
    "</div>";

export const skeletonFacetList = Array.from({ length: 5 })
    .map(() => facetSkeletonItem)
    .join("");
