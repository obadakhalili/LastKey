import { createRouter, createWebHistory } from "vue-router"

const vueRouter = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: "/signup",
      name: "Signup",
      component: () => import("@/pages/Signup.vue"),
    },
    // TODO: 404 page
  ],
})

export default vueRouter
