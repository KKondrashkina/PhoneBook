import Vue from "vue";
import VueRouter from "vue-router";
import PhoneBook from "../views/PhoneBook.vue";

Vue.use(VueRouter);

const routes = [
    {
        path: "/",
        name: "PhoneBook",
        component: PhoneBook
    }
];

const router = new VueRouter({
    mode: "history",
    base: process.env.BASE_URL,
    routes
});

export default router;
