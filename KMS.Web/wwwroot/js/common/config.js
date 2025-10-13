/**
 * @typedef {Object} AppConfig
 * @property {string} baseUrlApi
 * @property {string} baseUrlPublish
 * @property {string} baseUrlCore
 * @property {string} baseUrlFile
 * @property {string} baseUrlBlib
 * @property {string} appCode
 * @property {string} locationVal
 * @property {string} homeUrl
 * @property {string} loginUrl
 * @property {string} logoutUrl
 * @property {string} searchUrl
 * @property {string} searchCollectionUrl
 */

/** @type {AppConfig} */
const config = window.__APP_CONFIG__;

delete window.__APP_CONFIG__;
document.getElementById("app-config")?.remove();

export default config;
