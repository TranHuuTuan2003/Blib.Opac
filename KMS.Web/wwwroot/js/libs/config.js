var Config = (function () {
    async function FormSearchList(fileName) {
        return await $.getJSON(root_url_web + "json/" + client_site + "/" + lang + "/config/uc_form_search_list/" + fileName);
    }
    async function Form(fileName) {
        return await $.getJSON(root_url_web + "json/" + client_site + "/" + lang + "/config/uc_form/" + fileName);
    }
    async function Action(fileName) {
        return await $.getJSON(root_url_web + "json/" + client_site + "/" + lang + "/config/uc_action/" + fileName);
    }
    async function FormReportTemplate(fileName) {
        return await $.getJSON(root_url_web + "json/" + client_site + "/" + lang + "/report/" + fileName);
    }
    async function MessageData() {
        return await $.getJSON(root_url_web + "json/" + client_site + "/" + lang + "/config/data/message.json");
    }
    async function JsonData(fileName) {
        return await $.getJSON(root_url_web + "json/" + client_site + "/" + lang + "/config/data/" + fileName);
    }
    return { FormSearchList, Form, Action, FormReportTemplate, JsonData, MessageData};
})
