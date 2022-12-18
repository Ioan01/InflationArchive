import { useGlobalStore } from '../store/global';
<template lang="html">
    <div>
        <h2 class="text-center mt-5">Welcome to Inflation Archive!</h2>
    </div>
</template>
<script lang="ts">import { useGlobalStore } from '@/store/global';


export default {
    setup() {

        const store = useGlobalStore()
        store
    }

}
</script>
<style lang="">
    
</style>