import Vue from "vue";
import Vuex from "vuex";
import axios from "axios";

Vue.use(Vuex);

export default new Vuex.Store({
    state: {
        contacts: [],
        isChange: false,
        searchString: ""
    },
    mutations: {
        setContacts(state, data) {
            state.contacts = data;
        },
        setIsChange(state, data) {
            state.isChange = data;
        },
    },
    actions: {
        getContacts({ commit, state }) {
            axios.get("/api/GetContacts?searchString=" + state.searchString).then(response => {
                commit("setContacts", response.data);
                return Promise.resolve();
            });

        },
        addContact({ commit, state }, request) {
            axios.post("/api/AddContact", request).then(response => {
                commit("setIsChange", response.data);
            });
        },
        deleteContact({ commit, state }, id) {
            return axios.post("/api/DeleteContact", id,
                {
                    headers: {
                        "Content-Type": "application/json"
                    }
                }).then(response => {
                    commit("setIsChange", response.data);
                });
        },
        deleteContacts({ commit, state }, ids) {
            return axios.post("/api/DeleteContacts", ids).then(response => {
                commit("setIsChange", response.data);
            });
        }
    },
    modules: {
    }
});
