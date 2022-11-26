import { createRouter, createWebHistory } from "vue-router"

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: "/signup",
      name: "Signup",
      meta: {
        private: false,
      },
      component: () => import("@/pages/Signup.vue"),
    },
    {
      path: "/login",
      name: "Login",
      meta: {
        private: false,
      },
      component: () => import("@/pages/Login.vue"),
    },
    {
      path: "/",
      name: "Home",
      meta: {
        private: true,
      },
      component: () => import("@/pages/DoorLocking.vue"),
    },
    // TODO: 404 page
  ],
})

export default router
