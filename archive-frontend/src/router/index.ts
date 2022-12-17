import LoginViewVue from "@/views/LoginView.vue";
import RegisterViewVue from "@/views/RegisterView.vue";
import Vue from "vue";
import VueRouter from "vue-router";
import HomeView from "../views/HomeView.vue";
import UserViewVue from "@/views/UserView.vue";
import ProductsViewVue from "@/views/ProductsView.vue";
Vue.use(VueRouter);

const routes = [
  {
    path: "/",
    name: "home",
    component: HomeView,
  },

  {
    path: "/register",
    name: "register",
    component: RegisterViewVue,
  },
  {
    path: "/login",
    name: "login",
    component: LoginViewVue,
  },
  {
    path: "/userPage",
    name: "user",
    component: UserViewVue,
  },
  {
    path: "/products",
    name: "products",
    component: ProductsViewVue
  }
  // {
  //   path: '/about',
  //   name: 'about',
  //   // route level code-splitting
  //   // this generates a separate chunk (about.[hash].js) for this route
  //   // which is lazy-loaded when the route is visited.
  //   component: () => import(/* webpackChunkName: "about" */ '../views/AboutView.vue')
  // }
];

const router = new VueRouter({
  routes,
});

export default router;
