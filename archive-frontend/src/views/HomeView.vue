import { useGlobalStore } from '../store/global';
<template lang="html">
    <div>
        <span>home</span>
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