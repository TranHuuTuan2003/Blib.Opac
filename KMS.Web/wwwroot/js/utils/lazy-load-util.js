export function createLazyLoadIntersectionObserve(imageSelector) {
    const lazyImages = document.querySelectorAll(imageSelector + ":not([src])");

    const observer = new IntersectionObserver(
        (entries, observer) => {
            entries.forEach((entry) => {
                if (entry.isIntersecting) {
                    const img = entry.target;
                    img.src = img.dataset.src;
                    img.classList.remove("lazy-load");
                    observer.unobserve(img);
                }
            });
        },
        { rootMargin: "0px 0px 50px 0px" }
    );

    lazyImages.forEach((img) => observer.observe(img));
}
