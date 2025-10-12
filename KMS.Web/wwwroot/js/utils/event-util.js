const eventManagers = new Map();

export function createEventManager(eventType, target = document) {
    if (!eventManagers.has(eventType)) {
        const callbacks = new Set();
        let initialized = false;

        function handler(e) {
            for (const cb of callbacks) {
                try {
                    cb(e);
                } catch (err) {
                    console.error(`${eventType} callback error:`, err);
                }
            }
        }

        eventManagers.set(eventType, {
            add(cb) {
                if (typeof cb === "function") callbacks.add(cb);
            },
            remove(cb) {
                callbacks.delete(cb);
            },
            clear() {
                callbacks.clear();
            },
            init() {
                if (!initialized) {
                    target.addEventListener(eventType, handler);
                    initialized = true;
                }
            },
        });

        eventManagers.get(eventType).init();
    }

    return eventManagers.get(eventType);
}

export function createDomEvent(eventType, target, callback) {
    const manager = createEventManager(eventType);

    manager.add((e) => {
        if (typeof target === "string") {
            if (e.target && e.target.closest) {
                const targetEl = e.target.closest(target);
                if (targetEl) callback(targetEl, e);
            }
        } else if (
            target instanceof Element ||
            target instanceof Document ||
            target instanceof Window
        ) {
            if (e.target === target || target.contains?.(e.target)) {
                callback(e.target, e);
            }
        }
    });
}
