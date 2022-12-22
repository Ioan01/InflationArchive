import { useGlobalStore } from '../store/global';
<template lang="html">
    <div class="container" style="max-width: 90%">
        <div v-if="token">
            <h2>Here are your favorite products:</h2>

            <div class="d-flex flex-wrap mx-auto">
                <ol class="d-flex flex-wrap">
                    <li v-for="product in favoriteProducts" :key="product.id" class="ma-5">
                        <v-card class="mt-5 " width="344">
                            <v-img :src="product.imageUri" width="344" height="200px"></v-img>

                            <v-card-title>
                                {{ product.name }}
                            </v-card-title>
                            {{ product.pricePerUnit }} LEI / {{ product.unit }}
                            <v-card-subtitle>
                            </v-card-subtitle>
                            <v-row class="justify-center mx-auto pa-3">
                                {{ product.category }}
                            </v-row>
                            <v-icon v-if="product.isFavoritedByCurrentUser" color="red"
                                @click="unfavoriteProduct(product.id)">mdi-heart</v-icon>
                            <v-icon v-else-if="token != ''" color="grey"
                                @click="favoriteProduct(product.id)">mdi-heart-outline</v-icon>
                            <v-spacer />
                            <router-link :to="{ name: 'productView', params: { productId: product.id } }"
                                class="router-link">
                                <v-btn color="primary">Details</v-btn>
                            </router-link>
                        </v-card>
                    </li>
                </ol>

            </div>

        </div>
        <h2 class="text-center mt-5">We have the following products you can browse:</h2>
        <h3 class="text-center mt-5">You can filter the products using the following criteria:</h3>
        <div class="d-flex mt-5">
            <v-select label="Choose category" @change="fetchProducts()" v-model="selectedCategory"
                :items="MegaImageCategories" />
            <v-spacer></v-spacer>
            <v-text-field label="Search by name" @change="fetchProducts()" v-model="name"></v-text-field>
            <v-spacer></v-spacer>
            <v-text-field label="Search by manufacturer" @change="fetchProducts()"
                v-model="manufacturer"></v-text-field>
        </div>
        <br>
        <h3 class="text-center mt-5">You can order the products by:</h3>
        <div class="d-flex mt-5">
            <v-select label="Order by" @change="fetchProducts()" v-model="selectedCriteria" :items="OrderCriterias" />
            <v-spacer></v-spacer>
            <v-select label="Order" @change="fetchProducts()" v-model="selectedOrder" :items="OrderAscDesc" />
            <v-spacer></v-spacer>
            <v-btn color="primary" @click="resetFilters()">Reset filters</v-btn>
            <v-spacer></v-spacer>
        </div>
        <div class="d-flex flex-wrap mx-auto">
            <ol class="d-flex flex-wrap">
                <li v-for="product in products" :key="product.id" class="ma-5">
                    <v-card class="mt-5 " width="344">
                        <v-img :src="product.imageUri" width="344" height="200px"></v-img>

                        <v-card-title>
                            {{ product.name }}
                        </v-card-title>
                        {{ product.pricePerUnit }} LEI / {{ product.unit }}
                        <v-card-subtitle>
                        </v-card-subtitle>
                        <v-row class="justify-center mx-auto pa-3">
                            {{ product.category }}
                        </v-row>
                        <v-icon v-if="product.isFavoritedByCurrentUser" color="red"
                            @click="unfavoriteProduct(product.id)">mdi-heart</v-icon>
                        <v-icon v-else-if="token != ''" color="grey"
                            @click="favoriteProduct(product.id)">mdi-heart-outline</v-icon>
                        <v-spacer />
                        <router-link :to="{ name: 'productView', params: { productId: product.id } }"
                            class="router-link">
                            <v-btn color="primary">Details</v-btn>
                        </router-link>
                    </v-card>
                </li>
            </ol>
            <v-spacer></v-spacer>
            <v-spacer></v-spacer>
            <v-pagination v-model="page" :length="Math.ceil(totalProducts / 20)"
                @input="changePage($event)"></v-pagination>
        </div>

    </div>
</template>
<script lang="ts">import { useGlobalStore } from '@/store/global';
import { QueryResponseModel } from '@/models/QueryResponseModel';
import axios from 'axios';
import { defineComponent, ref } from 'vue';
import { address } from '@/store/environment';
import { ProductModel } from '@/models/ProductModel';
import { storeToRefs } from 'pinia';

export default defineComponent({

    setup() {
        const { token } = storeToRefs(useGlobalStore());
        const products = ref<ProductModel[]>([]);

        const favoriteProducts = ref<ProductModel[]>([]);


        const totalProducts = ref<number>(0)
        const page = ref<number>(1)
        const name = ref("")
        const manufacturer = ref("")
        const selectedCategory = ref("")
        const selectedOrder = ref("")
        const selectedCriteria = ref("")
        const MegaImageCategories = ref([
            "Fructe/Legume",
            "Lactate/Oua",
            "Mezeluri",
            "Carne",
            "Peste si icre",
            "Semipreparate",
            "Produse congelate",
            "Cafea/Mic dejun",
            "Paine/Patiserie",
            "Zahar/Faina/Malai",
            "Orez/Legume uscate",
            "Paste",
            "Condimente",
            "Conserve",
            "Apa/Sucuri",
            "Alcool/Tutun",
            ""
        ]);
        const OrderCriterias = ref(["price", "name", ""]);
        const OrderAscDesc = ref(["asc", "desc", ""]);
        fetchProducts()
        resetFilters()

        async function fetchProducts() {

            try {
                let flags = "";
                if (name.value !== "") {
                    flags += "&name=" + name.value
                }

                if (manufacturer.value !== "") {
                    flags += "&manufacturer=" + manufacturer.value
                }

                if (selectedCategory.value !== "") {
                    flags += "&category=" + selectedCategory.value
                }

                if (selectedOrder.value !== "" && selectedCriteria.value !== "") {
                    flags += "&order=" + selectedOrder.value + "&orderBy=" + selectedCriteria.value
                }

                if (flags !== "") {
                    const response = await axios.get<QueryResponseModel>(
                        address() + '/product/getProducts?pagenr=' + (page.value - 1) + flags, {
                        headers: { Authorization: `Bearer ${token.value}` }
                    }
                    );
                    products.value = response.data.products
                    totalProducts.value = response.data.totalCount
                } else {
                    const response = await axios.get<QueryResponseModel>(
                        address() + '/product/getProducts?pagenr=' + (page.value - 1), {
                        headers: { Authorization: `Bearer ${token.value}` }
                    }
                    );
                    products.value = response.data.products
                    totalProducts.value = response.data.totalCount
                }

                if (token.value != '')
                    getFavoriteProducts(flags)


                console.log(products.value);


            } catch (error) {
                console.error(error);
            }
        }

        async function getFavoriteProducts(flags: string) {
            const response = await axios.get<QueryResponseModel>(
                address() + '/product/getFavorites?' + flags, {
                headers: { Authorization: `Bearer ${token.value}` }
            }

            )

            favoriteProducts.value = response.data.products

        }

        async function changePage($event: number) {
            page.value = $event
            fetchProducts()
        }

        async function resetFilters() {
            name.value = ""
            manufacturer.value = ""
            selectedCategory.value = ""
            selectedOrder.value = ""
            selectedCriteria.value = ""
            page.value = 1
            fetchProducts()
        }

        return {
            token,
            products,
            totalProducts,
            favoriteProducts,
            page,
            changePage, name, fetchProducts,
            MegaImageCategories, selectedCategory,
            manufacturer, selectedOrder, selectedCriteria,
            OrderCriterias, OrderAscDesc, resetFilters
        };
    },
    methods: {
        async favoriteProduct(productId: string) {



            try {

                const form = new FormData()
                form.append("productId", productId)
                await axios.post(address() + '/account/addfavorite', form, {
                    headers: { Authorization: `Bearer ${this.token}` }
                });
                this.fetchProducts();
            } catch (error) {
                console.error(error);
            }
        },
        async unfavoriteProduct(productId: string) {
            try {
                const form = new FormData()
                form.append("productId", productId)
                await axios.post(
                    address() + '/account/removefavorite', form, {
                    headers: { Authorization: `Bearer ${this.token}` }
                }
                );
                this.fetchProducts();
            } catch (error) {
                console.error(error);
            }
        }
    },
});
</script>
<style lang="">
    
</style>