import { useGlobalStore } from '../store/global';
<template lang="html">
    <div class="container" style="max-width: 90%">
        <div v-if="loggedIn === true">
            <h2>Here are your favorite products:</h2>

        </div>
        <h2 class="text-center mt-5">We have the following products you can browse:</h2>
        <h3 class="text-center mt-5">You can filter the products using the following criteria:</h3>
        <div class="d-flex mt-5">
            <v-btn color="primary">store</v-btn>
            <v-spacer></v-spacer>
            <v-btn color="primary">Category</v-btn>
            <v-spacer></v-spacer>
            <v-btn color="primary">Price Ascending</v-btn>
            <v-spacer></v-spacer>
            <v-btn color="primary">Price Descending</v-btn>
        </div>
        <v-text-field @change="fetchProducts()" v-model="name"></v-text-field>
        <div class="d-flex flex-wrap mx-auto">
            <ol class="d-flex flex-wrap">
                <li v-for="product in products" :key="product.id" class="ma-5">
                    <v-card class="mt-5" max-width="344">
                        <v-img :src="product.imageUri" height="200px"></v-img>

                        <v-card-title>
                            {{ product.name }}
                        </v-card-title>
                        {{ product.pricePerUnit }} LEI / {{ product.unit }}
                        <v-card-subtitle>
                        </v-card-subtitle>
                        <v-row class="justify-center mx-auto pa-3">
                            {{ product.category }}
                        </v-row>
                    </v-card>
                </li>
            </ol>
            <v-pagination v-model="page" :length="totalProducts / 20" @input="changePage($event)"></v-pagination>
        </div>

    </div>
</template>
<script lang="ts">import { useGlobalStore } from '@/store/global';
import { QueryResponseModel } from '@/models/QueryResponseModel';
import axios from 'axios';
import { defineComponent, ref } from 'vue';
import { address } from '@/store/environment';
import { ProductModel } from '@/models/ProductModel';

export default defineComponent({



    setup() {
        const { loggedIn } = useGlobalStore();
        const products = ref<ProductModel[]>([]);
        const totalProducts = ref<number>(0)
        const page = ref<number>(0)
        const name = ref("a")

        fetchProducts()

        async function fetchProducts() {
            try {
                const response = await axios.get<QueryResponseModel>(
                    address() + '/product/getProducts?pagenr=' + page.value + "&name=" + name.value
                );
                products.value = response.data.products
                totalProducts.value = response.data.totalCount

                console.log(products.value);


            } catch (error) {
                console.error(error);
            }
        }

        async function changePage($event: number) {
            page.value = $event
            fetchProducts()
        }

        return {
            loggedIn,
            products,
            totalProducts,
            page,
            changePage, name, fetchProducts
        };
    },
    methods: {


    },
});
</script>
<style lang="">
    
</style>